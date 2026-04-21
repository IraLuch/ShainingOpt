using ShainingOpt.Models;
using ShainingOpt.ViewModels;

namespace ShainingOpt.Mappers
{
    public static class ProfileMapper
    {
        public static ProfileViewModel ToViewModel(User user)
        {
            return new ProfileViewModel
            {
                Id = user.Company.CompanyId,
                CompanyName = user.Company?.CompanyName,
                ContactPerson = user.Company?.ContactPerson,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Inn = user.Company?.Inn,
                Kpp = user.Company?.Kpp,
                Address = user.Company?.LegalAddress,

                Password = "",
                NewPassword = "",
                ConfirmPassword = ""
            };
        }
        public static ProfileViewModel FromUpdateModel(UpdateProfileDataViewModel model, User user)
        {
            return new ProfileViewModel
            {
                Id = user.Company.CompanyId,
                CompanyName = model.CompanyName,
                ContactPerson = model.ContactPerson,
                Email = model.Email,
                Phone = model.Phone,
                Inn = model.Inn,
                Kpp = model.Kpp,
                Address = model.Address
            };
        }

        public static void UpdateUser(User user, UpdateProfileDataViewModel model)
        {
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.Phone;
        }

        public static void UpdateCompany(User user, UpdateProfileDataViewModel model)
        {
            if (user.Company == null) return;

            user.Company.CompanyName = model.CompanyName;
            user.Company.ContactPerson = model.ContactPerson;
            user.Company.Inn = model.Inn;
            user.Company.Kpp = model.Kpp;
            user.Company.LegalAddress = model.Address;
        }
    }
}
