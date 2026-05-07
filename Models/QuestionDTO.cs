using Org.BouncyCastle.Asn1.Ocsp;

namespace FeedBack_APP.Models
{
    public class QuestionDTO
    {
        public int IdQuestionD { get; set; }
        public int IdQuestionG { get; set; }
        public string Question { get; set; }
        public int Written_Optionable { get; set; }  //1: WITTEN    0: OPTIONABLE
        public List<ResponseDTO> Responses { get; set; }
    }
}
