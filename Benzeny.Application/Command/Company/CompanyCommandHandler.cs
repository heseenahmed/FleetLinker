using AutoMapper;
using Benzeny.Application.Common;
using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto;
using Benzeny.Domain.IRepository;
using BenzenyMain.Application.Command.Log;
using BenzenyMain.Application.Common;
using BenzenyMain.Domain.Entity.Dto.Company;
using BenzenyMain.Domain.Entity.Dto.Log;
using BenzenyMain.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BenzenyMain.Application.Command.Company
{
    public class CompanyCommandHandler :
        IRequestHandler<CreateCompanyCommand, bool>,
        IRequestHandler<UpdateCompanyCommand, APIResponse<bool>>,
        IRequestHandler<SwitchActiveCompanyCommand, APIResponse<object>>,
        IRequestHandler<DeleteCompanyCommand, APIResponse<object>>
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IMailRepository _mailRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;
        public CompanyCommandHandler(
            ICompanyRepository companyRepo,
            IMailRepository mailRepo,
            IWebHostEnvironment env,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            IMediator mediator)
        {
            _companyRepo = companyRepo;
            _mailRepo = mailRepo;
            _env = env;
            _mapper = mapper;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        public async Task<bool> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto ?? throw new ArgumentException("Invalid company creation request.");

            var company = _mapper.Map<Domain.Entity.Company>(dto);
            company.CreatedDate = DateTime.UtcNow;
            company.CreatedBy = Guid.NewGuid().ToString();
            company.IsActive = true;

            await using var transaction = await _companyRepo.BeginTransactionAsync(cancellationToken);

            try
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

                if (dto.Files?.Any() == true)
                    company.FilePaths = await IFileHelper.SaveFilesAsync(dto.Files, uploadFolder, baseUrl);

                if (dto.CompanyPicture != null)
                {
                    var profile = await IFileHelper.SaveFilesAsync(new List<IFormFile> { dto.CompanyPicture }, uploadFolder, baseUrl);
                    company.ProfilePicturePath = profile.FirstOrDefault();
                }

                await _companyRepo.AddAsync(company, cancellationToken);

                await NotifyAdminAsync(company);
                await NotifyCompanyAsync(company);

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                // cleanup uploaded files if we added any
                if (company.FilePaths != null)
                    IFileHelper.DeleteFiles(company.FilePaths);

                if (!string.IsNullOrEmpty(company.ProfilePicturePath))
                    IFileHelper.DeleteFiles(new List<string> { company.ProfilePicturePath });

                // bubble up to middleware
                throw new ApplicationException("Error occurred while creating the company.", ex);
            }
        }

        private async Task NotifyAdminAsync(Domain.Entity.Company company)
        {
            if (company == null)
                throw new ArgumentException("Company data is required for admin notification.");

            var path = Path.Combine(_env.WebRootPath, "Template", "AdminCompanyNotification.html");
            if (!File.Exists(path))
                throw new FileNotFoundException("Admin email template not found.", path);

            var template = await File.ReadAllTextAsync(path);
            template = template.Replace("[companyName]", company.Name)
                               .Replace("[companyEmail]", company.CompanyEmail)
                               .Replace("[companyPhone]", company.CompanyPhone)
                               .Replace("[submittedAt]", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            await _mailRepo.SendEmailAsync("benzenydev@gmail.com", "New Company Registered – Benzeny", template);
        }
        private async Task NotifyAdminAfterUpdateAsync(Domain.Entity.Company company)
        {
            if (company == null)
                throw new ArgumentException("Company data is required for admin notification.");

            var path = Path.Combine(_env.WebRootPath, "Template", "AdminCompanyUpdateNotification.html");
            if (!File.Exists(path))
                throw new FileNotFoundException("Admin email template not found.", path);

            var template = await File.ReadAllTextAsync(path);
            template = template.Replace("[companyName]", company.Name)
                               .Replace("[ownerFullName]", company.Users.Where(x => x.UserRoles.Any(x => x.Role.Name == "CompanyOwner")).FirstOrDefault()!.FullName)
                               .Replace("[ownerEmail]", company.Users.Where(x => x.UserRoles.Any(x => x.Role.Name == "CompanyOwner")).FirstOrDefault()!.Email)
                               .Replace("[approvedAt]", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            await _mailRepo.SendEmailAsync("benzenydev@gmail.com", "New Company Registered – Benzeny", template);
        }

        private async Task NotifyCompanyAsync(Domain.Entity.Company company)
        {
            if (company == null)
                throw new ArgumentException("Company data is required for company notification.");

            var path = Path.Combine(_env.WebRootPath, "Template", "CompanyValidation.html");
            if (!File.Exists(path))
                throw new FileNotFoundException("Company email template not found.", path);

            var template = await File.ReadAllTextAsync(path);
            template = template.Replace("[companyName]", company.Name)
                               .Replace("[companyEmail]", company.CompanyEmail)
                               .Replace("[companyPhone]", company.CompanyPhone)
                               .Replace("[URL]", "https://mainbenzeny.netlify.app/#/auth/boxed-signup/" + company.Id);

            await _mailRepo.SendEmailAsync(company.CompanyEmail, "Benzeny – Your company is Approved", template);
        }
        private async Task NotifyCompanyAfterUpdateAsync(Domain.Entity.Company company)
        {
            if (company == null)
                throw new ArgumentException("Company data is required for company notification.");

            var path = Path.Combine(_env.WebRootPath, "Template", "CompanyOwnerValidation.html");
            if (!File.Exists(path))
                throw new FileNotFoundException("Company email template not found.", path);

            var template = await File.ReadAllTextAsync(path);
            template = template.Replace("[companyName]", company.Name)
                               .Replace("[ownerFullName]", company.Users.Where(x=>x.UserRoles.Any(x=>x.Role.Name == "CompanyOwner")).FirstOrDefault()!.FullName)
                               .Replace("[ownerEmail]", company.Users.Where(x => x.UserRoles.Any(x => x.Role.Name == "CompanyOwner")).FirstOrDefault()!.Email)
                               .Replace("[url]", "https://mainbenzeny.netlify.app/#/auth/boxed-signin")
                               .Replace("[supportEmail]", "benzenydev@gmail.com");


            await _mailRepo.SendEmailAsync(company.Users.Where(x => x.UserRoles.Any(x => x.Role.Name == "CompanyOwner")).FirstOrDefault()!.Email, "Benzeny – Your CompanyOwner Data is Approved", template);
        }

        public async Task<APIResponse<bool>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _companyRepo.BeginTransactionAsync(cancellationToken);

            try
            {
                var dto = request.CompanyDto ?? throw new ArgumentException("Company update payload is required.");

                var company = await _companyRepo.GetByIdAsync(request.Id)
                              ?? throw new KeyNotFoundException("Company not found.");

                // CEO user handling
                var ceoUser = company.UserId != null
                    ? await _userManager.FindByIdAsync(company.UserId.ToString()!)
                    : null;

                var isNewUser = ceoUser == null;
                if (isNewUser)
                {
                    ceoUser = new ApplicationUser
                    {
                        FullName = dto.FullName,
                        PhoneNumber = dto.Mobile,
                        UserName = dto.Username,
                        CompanyId = company.Id,
                        Email = dto.Email,
                        SSN = dto.SSN,
                        RefreshToken = TokenGenerator.GenerateRefreshToken(),
                        RefreshTokenExpiryUTC = DateTime.UtcNow.AddDays(7),
                        Active = true
                    };

                    var createResult = await _userManager.CreateAsync(ceoUser, dto.Password);
                    if (!createResult.Succeeded)
                        throw new ApplicationException("Failed to create CEO user: " +
                            string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    
                }
                else
                {
                    ceoUser.FullName = dto.FullName;
                    ceoUser.PhoneNumber = dto.Mobile;
                    ceoUser.UserName = dto.Username;
                    ceoUser.Email = dto.Email;
                    ceoUser.SSN = dto.SSN;
                    ceoUser.RefreshToken = TokenGenerator.GenerateRefreshToken();
                    ceoUser.RefreshTokenExpiryUTC = DateTime.UtcNow.AddDays(7);
                    ceoUser.CompanyId = company.Id;

                    var updateRes = await _userManager.UpdateAsync(ceoUser);
                    if (!updateRes.Succeeded)
                        throw new ApplicationException("Failed to update CEO user: " +
                            string.Join(", ", updateRes.Errors.Select(e => e.Description)));
                }

                if (isNewUser)
                {
                    var roleResult = await _userManager.AddToRoleAsync(ceoUser, "CompanyOwner");
                    if (!roleResult.Succeeded)
                        throw new ApplicationException("User created but failed to assign CompanyOwner role: " +
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    
                }

                // company updates
                company.UserId = ceoUser.Id;
                company.Name = dto.Name ?? company.Name;
                company.Description = dto.Description ?? company.Description;
                company.CompanyEmail = dto.CompanyEmail ?? company.CompanyEmail;
                company.CompanyPhone = dto.CompanyPhone ?? company.CompanyPhone;
                company.UpdatedBy = request.UpdatedByUserId ?? Guid.Empty.ToString();
                company.UpdatedDate = DateTime.UtcNow;

                // files
                if (dto.Files?.Any() == true)
                {
                    if (company.FilePaths?.Any() == true)
                        IFileHelper.DeleteFiles(company.FilePaths);

                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    string baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

                    company.FilePaths = await IFileHelper.SaveFilesAsync(dto.Files, uploadPath, baseUrl);
                }

                var updated = await _companyRepo.UpdateCompanyAsync(company);
                if (!updated)
                    throw new ApplicationException("Failed to update company.");

                await transaction.CommitAsync(cancellationToken);
                if (isNewUser)
                { 
                    await NotifyCompanyAfterUpdateAsync(company);
                    await NotifyAdminAfterUpdateAsync(company);
                }
                return APIResponse<bool>.Success(true, "Company updated successfully.");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; // let middleware format the error
            }
        }

        public async Task<APIResponse<object>> Handle(SwitchActiveCompanyCommand request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var success = await _companyRepo.SwitchActiveCompanyAsync(request.CompanyId);

            var company = await _companyRepo.GetByIdAsync(request.CompanyId);
            // Determine new status for better logging message
            var actionStatus = success ? "Active" : "InActive";

            // ✅ Create log entry
            await _mediator.Send(new CreateLogCommand(new LogForCreateDto
            {
                Action = "SwitchActiveCompany",
                EntityName = "Company",
                EntityId = null, 
                PerformedBy = request.PerformedBy,
                Details = $"{company!.Name} Company status changed to {actionStatus} by {request.PerformedBy}."
            }), cancellationToken);

            return APIResponse<object>.Success(null, "Company status and related users updated successfully.");
        }

        public async Task<APIResponse<object>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            // 🏢 Get company info before deletion
            var company = await _companyRepo.GetByIdAsync(request.CompanyId);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            // 🗑️ Delete company
            var deleted = await _companyRepo.DeleteCompanyAsync(request.CompanyId);
            if (!deleted)
                throw new KeyNotFoundException("Company not found or delete failed.");

            // 🧾 Create log entry
            await _mediator.Send(new CreateLogCommand(new LogForCreateDto
            {
                Action = "DeleteCompany",
                EntityName = "Company",
                EntityId = null,
                PerformedBy = request.PerformedBy,
                Details = $"Company '{company.Name}' was deleted by {request.PerformedBy}."
            }), cancellationToken);

            return APIResponse<object>.Success(null, "Company and all users deleted successfully.");

        }
    }
}
