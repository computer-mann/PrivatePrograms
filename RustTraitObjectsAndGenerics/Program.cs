﻿namespace RustTraitObjectsAndGenerics
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var components = new Draw[] { new Circle(), new Button() };
            var screen = new Screen();
            foreach (var component in components) {
                screen.display(component);
            }
            //var components = new Draw[] { new Circle(), new Button() };
            //var screen = new Screen<Circle>();
            //foreach (var component in components)
            //{
            //    screen.display(component);
            //}

            var average = new int[] {  };
            Console.WriteLine(average.Average());
        }
    }
    interface Draw {
        void draw();
    }
    class Screen {
        public void display(Draw d) {
            d.draw();
        }
    }
    //class Screen<T> where T : Draw
    //{
    //    public void display(T d)
    //    {
    //        d.draw();
    //    }
    //}
    class Circle : Draw {
        public void draw() {
            Console.WriteLine("Circle");
        }
    }
    class Button: Draw {
        public void draw() {
            Console.WriteLine("Button");
        }
    }
}
