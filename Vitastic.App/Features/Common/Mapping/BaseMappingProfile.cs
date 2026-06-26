using AutoMapper;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Common.Mapping;

public class BaseMappingProfile : Profile
{
    /// <summary>
    /// This class used for map Value Objects
    /// </summary>
    public BaseMappingProfile()
    {
        #region Base

        CreateMap<FullName, string>().ConvertUsing(src => src.Value);
        CreateMap<Title, string>().ConvertUsing(src => src.Value);
        CreateMap<FullName?, string>().ConvertUsing(src =>src ==null ? string.Empty : src.Value);
        CreateMap<FirstName, string>().ConvertUsing(src => src.Value);
        CreateMap<LastName, string>().ConvertUsing(src => src.Value);
        CreateMap<Email, string>().ConvertUsing(src => src.Value);
        CreateMap<PhoneNumber, string>().ConvertUsing(src => src.Value);
        CreateMap<PhoneNumber?, string>().ConvertUsing(src => src == null ? string.Empty: src.Value);
        CreateMap<PaymentGateway, string>().ConvertUsing(src => src.Value);
        CreateMap<PaymentGateway?, string>().ConvertUsing(src => src == null ? string.Empty: src.Value);
        CreateMap<Address, string>().ConvertUsing(src => src.GetFullAddress());
        CreateMap<Address?, string>().ConvertUsing(src => src ==null ? string.Empty : src.GetFullAddress());

        CreateMap<Slug, string>().ConvertUsing(src => src.Value);
        CreateMap<Description, string>().ConvertUsing(src => src.Value);
        CreateMap<Description?, string>().ConvertUsing(src => src ==null ? string.Empty : src.Value);
        CreateMap<ShortDescription, string>().ConvertUsing(src => src.Value);
        CreateMap<Money, decimal>().ConvertUsing(src => src.Value);
        CreateMap<Money, string>().ConvertUsing(src => src.Currency);
        CreateMap<Currency, string>().ConvertUsing(src => src.Code);
        //CreateMap<(PaginatedResult<>)>()
        #endregion

        #region User
        CreateMap<UserAvatar, string>().ConvertUsing(src => src.FileName);
        CreateMap<UserName, string>().ConvertUsing(src => src.Value);
        CreateMap<UserId, Guid>().ConvertUsing(src => src.Value);
        #endregion

        #region Cart
        #endregion
        #region Role

        CreateMap<RoleId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<RoleName, string>().ConvertUsing(src => src.Value);

        #endregion

        #region Persmission

        CreateMap<Permission, string>().ConvertUsing(src => src.Code);
        CreateMap<Permission, string>().ConvertUsing(src => src.Description);

        #endregion

        #region Course

        CreateMap<CourseId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<SectionId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<EpisodeId, Guid>().ConvertUsing(src => src.Value);

        CreateMap<SectionTitle, string>().ConvertUsing(src => src.Value);
        CreateMap<EpisodeVideoName, string>().ConvertUsing(src => src.Value);
        CreateMap<EpisodeVideoName?, string>().ConvertUsing(src => src==null ? string.Empty : src.Value);

        CreateMap<EpisodeTitle, string>().ConvertUsing(src => src.Value);

        CreateMap<CourseTitle, string>().ConvertUsing(src => src.Value);
        CreateMap<ShortDescription, string>().ConvertUsing(src => src.Value);
        CreateMap<CourseImageName, string>().ConvertUsing(src => src.Value);
        CreateMap<CourseImageName?, string>().ConvertUsing(src =>src ==null ? string.Empty : src.Value);
        CreateMap<CourseThumbnailName, string>().ConvertUsing(src => src.Value);
        CreateMap<CourseThumbnailName?, string>().ConvertUsing(src =>src ==null ? string.Empty : src.Value);
        CreateMap<CourseVideoName, string>().ConvertUsing(src => src.Value);
        CreateMap<CourseVideoName?, string>().ConvertUsing(src => src ==null ? string.Empty : src.Value);

        #endregion

        #region Instructor

        CreateMap<InstructorId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<InstructorBio, string>().ConvertUsing(src => src.Value);
        CreateMap<InstructorExpertise, string>().ConvertUsing(src => src.Value);
        CreateMap<InstructorSkills, List<string>>()
            .ConvertUsing(i => i.Values.Select(x => x.Value).ToList());

        #endregion

        #region Category

        CreateMap<CategoryId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<CategoryId?, Guid?>().ConvertUsing(src => src!.Value);
        CreateMap<CategoryName, string>().ConvertUsing(src => src.Value);

        #endregion

        #region Tag

        CreateMap<TagId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<TagId?, Guid?>().ConvertUsing(src => src!.Value);
        CreateMap<TagName, string>().ConvertUsing(src => src.Value);

        #endregion

        #region Order

        CreateMap<OrderId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<OrderNumber, string>().ConvertUsing(src => src.Value);

        CreateMap<OrderItemId, Guid>().ConvertUsing(src => src.Value);

        #endregion

        #region Discount

        CreateMap<DiscountId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<DiscountCode, string>().ConvertUsing(src => src.Value);
        CreateMap<DiscountCode?, string>().ConvertUsing(src => src == null ? string.Empty: src.Value);

        #endregion

        #region Pay Transaction

        CreateMap<PaymentTransactionId, Guid>().ConvertUsing(src => src.Value);
        CreateMap<PaymentInfo, string>().ConvertUsing(src => src.Authority);
        CreateMap<PaymentInfo, int>().ConvertUsing(src => src.RefId);
        CreateMap<PaymentInfo, DateTimeOffset?>().ConvertUsing(src => src.PaidDate);

        #endregion

        #region Permission
        CreateMap<PermissionId, Guid>().ConvertUsing(src => src.Value);
        #endregion
        #region Wallet

        CreateMap<WalletId, Guid>().ConvertUsing(src => src.Value);

        #endregion

        #region Money
        CreateMap<Money, decimal>().ConvertUsing(src => src.Value);
        CreateMap<Money, string>().ConvertUsing(src => src.ToString());
        #endregion
    }
}
