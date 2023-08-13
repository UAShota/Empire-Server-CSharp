using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.VersionOld
{
    class Answer
    {
        private List<string> names = new List<string>() { "Adolf", "Ben", "Cisco" };
        List<Person> aPerson = new List<Person>();
        List<Person> bPerson = new List<Person>();
        List<Person> cPerson = new List<Person>();

        public void Run()
        {

        }

        public void SortList()
        {
            foreach (string name in names)
            {
                AddToCharArray(new Person(name));
            }
        }


        private void AddToCharArray(Person person) {
            switch (person.nameChares[0]) {
                case 'A':
                    aPerson.Add(person);
                    break;
                case 'B':
                    bPerson.Add(person);
                    break;
                case 'C':
                    cPerson.Add(person);
                    break;
            }
        }
    }
    class Person {
       public string name;
      public  char[] nameChares;

        public Person(string name) {
            this.name = name;
            nameChares = name.ToCharArray();
        }
    }
}
