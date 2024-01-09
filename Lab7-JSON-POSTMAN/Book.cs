/**       
 * -------------------------------------------------------------------
 * 	   File name: Book.cs
 * 	Project name: Deserialization
 * -------------------------------------------------------------------
 *  Author’s name and email:    Michael Ng, ngmw01@etsu.edu			
 *            Creation Date:	04/05/2022	
 *            Last Modified:    04/06/2022
 * -------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Newtonsoft
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Description { get; set; }
        public string? SelfLink { get; set; }


        /*
         * The base constructor of the Book class.
         * Doesn't intialize anything.
         * 
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/06/2022
         */
        public Book() 
        {
        
        }

        /*
         * The primary constructor of the Book class.
         * Initializes all fields in the class.
         * 
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/06/2022
         * @param string Id, Title, Description, SelfLink
         * @param List<string> Authors
         */
        public Book(string Id, string Title, List<string> Authors, string Description, string? SelfLink = null) 
        {
            this.Id = Id;
            this.Title = Title;
            this.Authors = Authors;
            this.Description = Description;
            this.SelfLink = SelfLink;
        }

        /*
         * The overriding ToString method of the Book class.
         * Returns a formatted string.
         * 
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/06/2022
         */
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("Book ID: " + Id + "\n");
            sb.Append("Book Title: " + Title + "\n--------------------\n");
            sb.Append("Authors: \n");
            if (Authors != null && Authors.Count > 0)
            {
                foreach (var author in Authors)
                {
                    sb.Append(" - " + author + "\n");
                }
            }
            else 
            {
                sb.Append(" - No Authors.\n");
            }


            sb.Append("\n--------------------\nDescription: " + Description + "\n");
            sb.Append("Self Link: " + SelfLink + "\n");
            return sb.ToString();
        }
    }
}
