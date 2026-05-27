using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using WebWerverPart.Models;
using WebWerverPart.Models.DTO;

namespace WebWerverPart.Services
{
    public class MainService
    {
        private readonly IvanvisionDbContext _db;

        public MainService(IvanvisionDbContext db, JWTService jwtService)
        {
            _db = db;
        }

        public async Task<(bool success, string Emessage)> SendBugReportAsync(ReportDTO reportDTO, int user_id)
        {
            if (string.IsNullOrWhiteSpace(reportDTO.Description))
                return (false, "Message is empty");

            if (reportDTO.Description.Length > 500)
                return (false, "Message is longer than 500");

            var bugReport = new BugReport
            {
                UserId = user_id,
                ReportDescription = reportDTO.Description,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            try
            {
                _db.BugReports.Add(bugReport);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return (false, "another server error");
            }

            return (true, "report has sent");
        }
    }
}
