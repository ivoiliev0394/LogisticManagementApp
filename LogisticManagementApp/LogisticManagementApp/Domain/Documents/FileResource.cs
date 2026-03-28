using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Documents
{
    public class FileResource : BaseEntity
    {
        /// <summary>
        /// Ключ към storage: локален път, blob key, S3 key и т.н.
        /// </summary>
        [Required]
        [MaxLength(400)]
        public string StorageKey { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string ContentType { get; set; } = "application/octet-stream";

        [Range(0, long.MaxValue)]
        public long SizeBytes { get; set; }

        [MaxLength(128)]
        public string? Checksum { get; set; } // SHA256, MD5

        [Required]
        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
