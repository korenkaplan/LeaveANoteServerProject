using LeaveANoteServerProject.Data;
using LeaveANoteServerProject.DTO_s.Accident_Dto_s;
using LeaveANoteServerProject.DTO_s.StatsDto_s;
using LeaveANoteServerProject.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LeaveANoteServerProject.Services.StatsService
{
    public class StatsService : IStatsService
    {
        private readonly DataContext _context;

        public StatsService(DataContext context)
        {
            _context = context;
        }

        public async Task<HttpResponse<RegisteredUsersDto>> RegisteredUsersData(RegisteredUserDataDtoReq registeredUserDataDtoReq)
        {
            try
            {
                if (registeredUserDataDtoReq.Role != "admin")
                    return new HttpResponse<RegisteredUsersDto> { IsSuccessful = false, Message = "Access Denied", Error = "This data is only visible to admin users" };

                int year = registeredUserDataDtoReq.Year;
                List<User> users = await _context.Users.Where(u => u.CreatedAt.Year == year).ToListAsync();

                Dictionary<string, int> dict = CreateMonthDictionary(year);
                RegisteredUsersDto registeredUsersDto = FillMonthlyDictionaryWithData(dict, users);

                return new HttpResponse<RegisteredUsersDto> { IsSuccessful = true, Message = "Graph data", Data = registeredUsersDto , StatusCode = 200};
            }
            catch (Exception ex)
            {
                return new HttpResponse<RegisteredUsersDto> { IsSuccessful = false, Message = "Failed to fetch graph data", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<ReportsDistributionDto>> ReportsDistributtion(ReportsDistributtionDtoReq reportsDistributtionDtoReq)
        {
            try
            {
                List<Accident> accidents = await _context.Accidents.ToListAsync();
                List<UnmatchedReport> unmatchedReports = await _context.UnmatchedReports.ToListAsync();

                ReportsDistributionDto reportsDistributionDto = FillDistributionData(accidents,unmatchedReports);

                return new HttpResponse<ReportsDistributionDto> { IsSuccessful = true, Message = "Graph data", Data = reportsDistributionDto, StatusCode = 200 };

            }
            catch (Exception ex)
            {
                return new HttpResponse<ReportsDistributionDto> { IsSuccessful = false, Message = "Failed to fetch graph data", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }
        #region Private Fucntions
        private ReportsDistributionDto FillDistributionData(List<Accident> accidents, List<UnmatchedReport> unmatchedReports)
        {
            ReportsDistributionDto reportsDistributionDto = new ReportsDistributionDto();
            AddNotesAndReportsCounters(accidents, reportsDistributionDto);
            AddUnmatchedReportsCounter(unmatchedReports, reportsDistributionDto);
            return reportsDistributionDto;
        }
        private void AddNotesAndReportsCounters(List<Accident> accidents, ReportsDistributionDto reportsDistributionDto)
        {
            int notesCounter = 0, reportsCounter = 0;
            foreach (Accident accident in accidents)
            {
                if (accident.Type == "note")
                    notesCounter++;
                else
                    reportsCounter++;
            }
            reportsDistributionDto.DistributionList.Add(new ReportDistributionItemDto { Category = "Notes", Count = notesCounter });
            reportsDistributionDto.DistributionList.Add(new ReportDistributionItemDto { Category = "Reports", Count = reportsCounter });
        }
        private void AddUnmatchedReportsCounter(List<UnmatchedReport> unmatchedReports, ReportsDistributionDto reportsDistributionDto)
        {
            int unMatchedReportCounter = 0;
            foreach (UnmatchedReport unmatched in unmatchedReports)
            {
                unMatchedReportCounter += unmatched.IsUnmatched == false ? 1 : 0;
            }
            reportsDistributionDto.DistributionList.Add(new ReportDistributionItemDto { Category = "Unmatched \n Reports", Count = unMatchedReportCounter });

        }

        private Dictionary<string, int> CreateMonthDictionary(int year)
        {
            int currantYear = DateTime.Now.Year;
            int currantMonth = DateTime.Now.Month;
            Dictionary<string, int> dict = new Dictionary<string, int>
        {
            { "Jan", 0 },
            { "Feb", 0 },
            { "Mar", 0 },
            { "Apr", 0 },
            { "May", 0 },
            { "Jun", 0 },
            { "Jul", 0 },
            { "Aug", 0 },
            { "Sep", 0 },
            { "Oct", 0 },
            { "Nov", 0 },
            { "Dec", 0 }
        };
            // Take the required number of elements (currentMonth)
            var takenElements = dict.Take(currantYear == year ? currantMonth : 12);

            // Convert the taken elements back into a SortedDictionary
            Dictionary<string, int> resultDict = takenElements.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return resultDict;
        }
        private RegisteredUsersDto FillMonthlyDictionaryWithData(Dictionary<string, int> monthlyDict, List<User> users)
        {
            int sum = 0;
            foreach (var u in users)
            {
                string shortMonthName = u.CreatedAt.ToString("MMM");
                monthlyDict[shortMonthName] += 1;
            }
            RegisteredUsersDto registeredUsersDto = new RegisteredUsersDto();
            foreach (var kvp in monthlyDict)
            {
                Log.Information($"{@kvp.Key},{@kvp.Value}", kvp);
                sum += kvp.Value;
                registeredUsersDto.MonthlyUsers.Add(new MonthlyUsersDto { Month = kvp.Key, Users = sum });
            }
            return registeredUsersDto;
        } 
        #endregion
    }
}
