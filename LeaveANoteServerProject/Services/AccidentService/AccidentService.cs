using LeaveANoteServerProject.Data;
using LeaveANoteServerProject.DTO_s.Accident_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;
using LeaveANoteServerProject.Services.UserService;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace LeaveANoteServerProject.Services.AccidentService
{
    public class AccidentService : IAccidentService
    {
        private readonly DataContext _context;

        public AccidentService(DataContext context)
        {
            _context = context;
        }

        public async Task<HttpResponse<Accident>> CreateNote(CreateNoteReqDto createNoteDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(createNoteDto.UserId);

                if (user == null)
                {
                    return new HttpResponse<Accident> { IsSuccessful = false, StatusCode = 500, Message = "There was a problem  in the connection of the server and the database" };
                }
                Accident acccident = CreateAccidentObjectFromNote(createNoteDto);
                user.Accidents.Add(acccident);
                await _context.SaveChangesAsync();
                return new HttpResponse<Accident> { IsSuccessful = true, StatusCode = 201, Data = acccident, Message = $"The note has been sent to {user.Name}" };
            }
            catch (Exception ex)
            {
                return new HttpResponse<Accident> { IsSuccessful = false, Message = "Failed to create note", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<Accident>> CreateReport(CreateReportReqDto createReportReqDto)
        {
            try
            {
                var user = await _context.Users.Where(u => u.CarNumber == createReportReqDto.DamagedCarNumber).FirstOrDefaultAsync();

                Accident accident = await CreateAccidentObjectFromReport(createReportReqDto);

                //if not found add to unmatched reports table
                if (user == null)
                {
                    UnmatchedReport unmatched = await CreateUnmatchedReport(accident, createReportReqDto.DamagedCarNumber);
                    await _context.UnmatchedReports.AddAsync(unmatched);
                    await _context.SaveChangesAsync();
                    return new HttpResponse<Accident> { IsSuccessful = true, Message = "The report has been Added to unmatched reoprts", Data = accident, StatusCode = 201 };
                }
                //if damaged user found add to the user's accident list
                user.Accidents.Add(accident);
                await _context.SaveChangesAsync();
                return new HttpResponse<Accident> { IsSuccessful = true, Message = $"The report has been sent to {user.Name}", Data = accident, StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<Accident> { IsSuccessful = false, Message = "Failed to create report", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        #region Functions for creating accident object and Unmatched report object

        /// <summary>
        /// Creates an <see cref="Accident"/> object based on the information provided in the <paramref name="createNoteReqDto"/> to be used for creating a new accident of type note.
        /// </summary>
        /// <param name="createNoteReqDto">The DTO (Data Transfer Object) containing the information to create the note (accident report).</param>
        /// <returns>The created <see cref="Accident"/> object.</returns>
        private Accident CreateAccidentObjectFromNote(CreateNoteReqDto createNoteReqDto)
        {
            Accident accident = new Accident();
            accident.HittingDriverName = createNoteReqDto.HittingDriverName;
            accident.HittingCarNumber = createNoteReqDto.HittingCarNumber;
            accident.HittingDriverPhoneNumber = createNoteReqDto.HittingDriverPhoneNumber;
            accident.ImageSource = createNoteReqDto.ImageSource;
            accident.Type = "note";
            accident.IsIdentify = true;
            return accident;
        }

        /// <summary>
        /// Creates an <see cref="Accident"/> object based on the information provided in the <paramref name="createReportReqDto"/> to be used for creating a new accident of type report.
        /// </summary>
        /// <param name="createReportReqDto">The DTO (Data Transfer Object) containing the information to create the accident report.</param>
        /// <returns>The created <see cref="Accident"/> object.</returns>
        private async Task<Accident> CreateAccidentObjectFromReport(CreateReportReqDto createReportReqDto)
        {
            //set the certain fields
            Accident accident = new Accident();
            accident.HittingCarNumber = createReportReqDto.HittingCarNumber;
            accident.ReporterName = createReportReqDto.Reporter.Name;
            accident.ReporterPhoneNumber = createReportReqDto.Reporter.PhoneNumber;
            accident.ImageSource = createReportReqDto.ImageSource;
            accident.Type = "report";
            accident.IsAnonymous = createReportReqDto.IsAnonymous;

            //search for the hitting driver And set IsIdentify to the result
            var offendingUser = await _context.Users.Where(u => u.CarNumber == createReportReqDto.HittingCarNumber).FirstOrDefaultAsync();

            //if offendign user in not in the system assign the following values
            if (offendingUser != null)
            {
                accident.HittingDriverName = offendingUser.Name;
                accident.HittingDriverPhoneNumber = offendingUser.PhoneNumber;
                accident.IsIdentify = true;
            }

            //if unmatched handle unmatched report
            return accident;
        }

        /// <summary>
        /// Creates an <see cref="UnmatchedReport"/> object based on the provided <paramref name="accident"/> and the damaged car number.
        /// </summary>
        /// <param name="accident">The <see cref="Accident"/> object associated with the unmatched report.</param>
        /// <param name="carNumber">The damaged car number for the unmatched report.</param>
        /// <returns>The created <see cref="UnmatchedReport"/> object.</returns>
        private async Task<UnmatchedReport> CreateUnmatchedReport(Accident accident, string carNumber)
        {
            UnmatchedReport unmatched = new UnmatchedReport();
            unmatched.Accident = accident;
            unmatched.DamagedCarNumber = carNumber;
            return unmatched;
        } 
        #endregion
    }
}
