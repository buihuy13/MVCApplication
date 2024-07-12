namespace MVCApplication.Models
{
    public class SummernoteModel
    {
        public string IdEditor { get; set; }
        public bool loadLibrary { get; set; }
        public int height { get; set; } = 120;
        public string toolbar { get; set; } = "[\r\n                ['style', ['style']],\r\n                ['font', ['bold', 'underline', 'clear']],\r\n                ['color', ['color']],\r\n                ['para', ['ul', 'ol', 'paragraph']],\r\n                ['table', ['table']],\r\n                ['insert', ['link', 'picture', 'video']],\r\n                ['height', ['height']],\r\n                ['view', ['fullscreen', 'codeview', 'help']]\r\n            ]";
        public SummernoteModel(string IdEditor, bool loadLibrary = true)
        {
            this.IdEditor = IdEditor;
            this.loadLibrary = loadLibrary;
        }
    }
}
