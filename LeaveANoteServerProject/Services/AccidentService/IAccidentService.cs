using LeaveANoteServerProject.DTO_s.Accident_Dto_s;

namespace LeaveANoteServerProject.Services.AccidentService
{
    public interface IAccidentService
    {
        /// <summary>
        /// Creates a new accident (type note) and associates it with the user based on the provided <paramref name="createNoteDto.UserId"/>.
        /// </summary>
        /// <param name="createNoteDto">The DTO (Data Transfer Object) containing the information to create the note.</param>
        /// <returns>A <see cref="HttpResponse{Accident}"/> with details about the success or failure of the operation and the created accident (note) object.</returns>
        Task<HttpResponse<Accident>> CreateNote(CreateNoteReqDto createNoteDto);

        /// <summary>
        /// Creates a new accident (type report) and associates it with the corresponding user based on the provided damaged car number <paramref name="createReportReqDto.DamagedCarNumber"/>.
        /// </summary>
        /// <param name="createReportReqDto">The DTO (Data Transfer Object) containing the information to create the accident report.</param>
        /// <returns>A <see cref="HttpResponse{Accident}"/> with details about the success or failure of the operation and the created accident object.</returns>
        Task<HttpResponse<Accident>> CreateReport(CreateReportReqDto createReportReqDto);
    }
}
