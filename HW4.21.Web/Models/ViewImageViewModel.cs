using HW4._21.Data;

namespace HW4._21.Web.Models
{
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public bool IsAuthroized { get; set; }
        public List<int> Ids { get; set; }
        public string Meessage { get; set; }
    }
}
