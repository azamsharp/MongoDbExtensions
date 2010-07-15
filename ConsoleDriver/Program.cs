using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;


namespace ConsoleDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            var mongo = new Mongo();
            mongo.Connect(); 


            
        }
    }
}
