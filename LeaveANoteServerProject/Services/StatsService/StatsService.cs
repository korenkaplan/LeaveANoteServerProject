using LeaveANoteServerProject.Data;
using LeaveANoteServerProject.DTO_s.Accident_Dto_s;
using LeaveANoteServerProject.DTO_s.StatsDto_s;
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

        public async Task<HttpResponse<List<MonthlyUsersDto>>> RegisteredUsersData(int year)
        {
            try
            {

                List<User> users = await _context.Users.Where(u => u.CreatedAt.Year == year).ToListAsync();

                Dictionary<string, int> dict = CreateMonthDictionary(year);
                RegisteredUsersDto registeredUsersDto = FillMonthlyDictionaryWithData(dict, users);

                return new HttpResponse<List<MonthlyUsersDto>> { Success = true, Message = "Graph data", Data = registeredUsersDto.MonthlyUsers , StatusCode = 200};
            }
            catch (Exception ex)
            {
                return new HttpResponse<List<MonthlyUsersDto>> { Success = false, Message = "Failed to fetch graph data", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<List<ReportDistributionItemDto>>> ReportsDistributtion()
        {
            try
            {
                List<Accident> accidents = await _context.Accidents.ToListAsync();
                List<UnmatchedReport> unmatchedReports = await _context.UnmatchedReports.ToListAsync();
                ReportsDistributionDto reportsDistributionDto = FillDistributionData(accidents,unmatchedReports);

                return new HttpResponse<List<ReportDistributionItemDto>> { Success = true, Message = "Graph data", Data = reportsDistributionDto.DistributionList, StatusCode = 200 };

            }
            catch (Exception ex)
            {
                return new HttpResponse<List<ReportDistributionItemDto>> { Success = false, Message = "Failed to fetch graph data", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        #region Private Fucntions
        /// <summary>
        /// Fills the distribution data for reports and unmatched reports.
        /// </summary>
        /// <param name="accidents">The list of accidents (reports).</param>
        /// <param name="unmatchedReports">The list of unmatched reports.</param>
        /// <returns>The filled distribution data.</returns>
        private ReportsDistributionDto FillDistributionData(List<Accident> accidents, List<UnmatchedReport> unmatchedReports)
        {
            ReportsDistributionDto reportsDistributionDto = new ReportsDistributionDto();
            AddNotesAndReportsCounters(accidents, reportsDistributionDto, unmatchedReports);
            AddUnmatchedReportsCounter(unmatchedReports, reportsDistributionDto);
            return reportsDistributionDto;
        }
        /// <summary>
        /// Adds the counters for notes and reports to the distribution data.
        /// </summary>
        /// <param name="accidents">The list of accidents (reports).</param>
        /// <param name="reportsDistributionDto">The distribution data to update.</param>
        private void AddNotesAndReportsCounters(List<Accident> accidents, ReportsDistributionDto reportsDistributionDto, List<UnmatchedReport> unmatchedReports)
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
        /// <summary>
        /// Adds the counter for unmatched reports to the distribution data.
        /// </summary>
        /// <param name="unmatchedReports">The list of unmatched reports.</param>
        /// <param name="reportsDistributionDto">The distribution data to update.</param>
        private void AddUnmatchedReportsCounter(List<UnmatchedReport> unmatchedReports, ReportsDistributionDto reportsDistributionDto)
        {
            int unMatchedReportCounter = 0;
            ReportDistributionItemDto reportsDistributions = reportsDistributionDto.DistributionList.FirstOrDefault(item => item.Category == "Reports");
            foreach (UnmatchedReport unmatched in unmatchedReports)
            {
                if(!unmatched.IsUnmatched)
                {
                    unMatchedReportCounter++;
                    reportsDistributions!.Count--; // remove the unmatched from the reports counter

                }
            }
            reportsDistributionDto.DistributionList.Add(new ReportDistributionItemDto { Category = "Unmatched \n Reports", Count = unMatchedReportCounter });

        }
        /// <summary>
        /// Creates a dictionary representing the months of the specified year.
        /// </summary>
        /// <param name="year">The year for which the dictionary is created.</param>
        /// <returns>A dictionary representing the months of the specified year.</returns>
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
        /// <summary>
        /// Fills the monthly dictionary with data based on the list of users.
        /// </summary>
        /// <param name="monthlyDict">The monthly dictionary to update.</param>
        /// <param name="users">The list of users.</param>
        /// <returns>The updated RegisteredUsersDto object with monthly data.</returns>
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
