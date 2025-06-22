using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Netflixx.Models
{
    public class FilmsModel
    {
        [Key]
        public int Id { get; set; }

        // Thông tin cơ bản
        [Required(ErrorMessage = "Vui lòng nhập tên phim.")]
        [Display(Name = "Tên phim")]
        [StringLength(200, ErrorMessage = "Tên phim không vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thể loại.")]
        [Display(Name = "Thể loại")]
        [StringLength(100, ErrorMessage = "Thể loại không vượt quá 100 ký tự")]
        public string Genre { get; set; }

        [Range(1, 500, ErrorMessage = "Thời lượng phải từ 1 đến 500 phút")]
        [Display(Name = "Thời lượng (phút)")]
        public int? Duration { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày phát hành")]
        public DateTime? ReleaseDate { get; set; }

        [Range(0, 1000000, ErrorMessage = "Giá phải từ 0 đến 1,000,000")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá")]
        public decimal? Price { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Poster")]
        [Url(ErrorMessage = "Đường dẫn poster không hợp lệ")]
        public string PosterUrl { get; set; }

        [Display(Name = "Trailer")]
        [Url(ErrorMessage = "Đường dẫn trailer không hợp lệ")]
        public string TrailerUrl { get; set; }

        // Trạng thái (đã đổi thành string)
        [Display(Name = "Trạng thái")]
        [Column(TypeName = "nvarchar(20)")]
        public string Status { get; set; } = "Active"; // Giá trị mặc định

        // Độ tuổi (đã đổi thành string)
        [Display(Name = "Độ tuổi")]
        [Column(TypeName = "nvarchar(10)")]
        public string RatingValue { get; set; } = "P"; // Giá trị mặc định

        [Display(Name = "Đạo diễn")]
        [StringLength(100, ErrorMessage = "Tên đạo diễn không vượt quá 100 ký tự")]
        public string Director { get; set; }

        [Display(Name = "Diễn viên")]
        public string Actors { get; set; }

        [Display(Name = "URL phim")]
        [Url(ErrorMessage = "Đường dẫn phim không hợp lệ")]
        public string FilmURL { get; set; }


        [Display(Name = "Đánh giá")]
        [Range(0, 10, ErrorMessage = "Đánh giá phải từ 0 đến 10")]
        public float Rating { get; set; } = 0.0f;

        // Theo dõi thay đổi (đã đổi thành string)
        [Display(Name = "Hành động cuối")]
        [Column(TypeName = "nvarchar(20)")]
        public string? LastAction { get; set; }

        [Display(Name = "Người sửa cuối")]
        public string? ModifiedBy { get; set; }

        [Display(Name = "Ngày sửa cuối")]
        public DateTime? ModificationDate { get; set; }

        [Display(Name = "Trường đã sửa")]
        [Column(TypeName = "nvarchar(500)")]
        public string? ModifiedFields { get; set; }

        [Display(Name = "Ghi chú thay đổi")]
        [StringLength(500, ErrorMessage = "Ghi chú không vượt quá 500 ký tự")]
        public string? ChangeNote { get; set; }

        // Quan hệ
        public virtual ICollection<PackageFilmsModel> PackageFilms { get; set; } = new List<PackageFilmsModel>();
        public virtual ICollection<PromotionFilmsModel> PromotionFilms { get; set; } = new List<PromotionFilmsModel>();
        public virtual ICollection<FilmPurchasesModel> Purchases { get; set; } = new List<FilmPurchasesModel>();

        // Phương thức cập nhật thông tin thay đổi (đã sửa đổi)
        public void UpdateTrackingInfo(FilmsModel original, string action, string modifiedBy, string note = null)
        {
            var changedFields = new List<string>();
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (ShouldIgnoreProperty(prop.Name)) continue;

                var oldValue = prop.GetValue(original);
                var newValue = prop.GetValue(this);

                if (!Equals(oldValue, newValue))
                {
                    changedFields.Add(GetDisplayName(prop));
                }
            }

            ModifiedFields = changedFields.Any() ? string.Join(", ", changedFields) : null;
            LastAction = action;
            ModifiedBy = modifiedBy;
            ModificationDate = DateTime.Now;
            ChangeNote = note;
        }

        private bool ShouldIgnoreProperty(string propName)
        {
            var ignoreList = new List<string>
            {
                nameof(Id),
                nameof(LastAction),
                nameof(ModifiedBy),
                nameof(ModificationDate),
                nameof(ModifiedFields),
                nameof(ChangeNote),
                nameof(PackageFilms),
                nameof(PromotionFilms),
                nameof(Purchases)
            };

            return ignoreList.Contains(propName);
        }

        // Các phương thức tiện ích đã đơn giản hóa
        public string GetStatusText()
        {
            return GetDisplayText(Status, new Dictionary<string, string>
            {
                { "Active", "Đang chiếu" },
                { "Inactive", "Ngừng chiếu" },
                { "Upcoming", "Sắp chiếu" },
                { "Deleted", "Đã xóa" }
            });
        }

        public string GetRatingText()
        {
            return GetDisplayText(RatingValue, new Dictionary<string, string>
            {
                { "P", "Mọi lứa tuổi" },
                { "C13", "13+" },
                { "C16", "16+" },
                { "C18", "18+" }
            });
        }

        public string GetLastActionText()
        {
            return GetDisplayText(LastAction, new Dictionary<string, string>
            {
                { "Create", "Tạo mới" },
                { "Update", "Cập nhật" },
                { "Delete", "Xóa" },
                { "Restore", "Khôi phục" }
            });
        }

        private string GetDisplayText(string value, Dictionary<string, string> mapping)
        {
            return mapping.TryGetValue(value, out var displayText) ? displayText : value;
        }

        private string GetDisplayName(PropertyInfo prop)
        {
            var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.Name ?? prop.Name;
        }

        [ForeignKey("ProductionManagerId")]
        public virtual ProductionManager ProductionManager { get; set; }



    }
}