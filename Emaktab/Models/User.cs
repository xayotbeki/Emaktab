namespace Emaktab.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? user_fio { get; set; }
        public UsertType UsertType { get; set; }
        public string? user_type_name { get; set; }
        public int user_typeId { get; set; }
        public int? user_workplace { get; set; }
        public string? user_login { get; set; }
        public string? user_password { get; set; }
        public bool active { get; set; }
        public DateTime create_date { get; set; }


        public DateTime birthday { get; set; }
        public string? user_email { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public string Pinfl { get; set; }
        public string? Pasport_seria { get; set; }
        public long Pasport_number { get; set; }
        public string user_location { get; set; }
        public string user_level { get; set; }
        public string phone_number { get; set; }
        public int uzb_lang { get; set; }
        public int eng_lang { get; set; }
        public int rus_lang { get; set; }
        public int other_lang { get; set; }
        public int tajribasi { get; set; }
        public string Biography { get; set; }
        public string? IconPath { get; set; }
    }
}