using System;

namespace CinemaManagement.Common
{
    // Наследник от Film
    public class Cartoon : Film
    {
        public string Studio { get; set; }
        public bool Is3D { get; set; }

        public Cartoon(string title, string genre, int duration, string studio, bool is3D)
            : base(title, genre, duration)
        {
            Studio = studio;
            Is3D = is3D;
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Студия: {Studio}, 3D: {(Is3D ? "Да" : "Нет")}");
        }
    }
}
