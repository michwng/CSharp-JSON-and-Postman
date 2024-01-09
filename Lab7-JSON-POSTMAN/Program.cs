/**       
 * -------------------------------------------------------------------
 * 	   File name: Program.cs
 * 	Project name: Deserialization
 * -------------------------------------------------------------------
 *  Author’s name and email:    Michael Ng, ngmw01@etsu.edu			
 *            Creation Date:	04/05/2022	
 *            Last Modified:    04/07/2022
 * -------------------------------------------------------------------
 */

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

/*
 * This version of Lab 7 utilizes
 * the Newtonsoft.json NuGet package.
 * 
 * I did get similar (the same) results, 
 * and I think I like it better than
 * JsonSerializer.
 */
namespace Newtonsoft
{
    /*
     * Program runs the Console Application through the Main method.
     */
    public static class Program
    {
        //DefaultJsonPath navigates to the JsonData Directory in the Project's root folder.
        static string DefaultJsonPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString() + Path.DirectorySeparatorChar + "JsonData" + Path.DirectorySeparatorChar;
        static List<string> SelfLinks = new();
        static List<string> Contents = new();
        static List<Book> books = new();

        private static readonly HttpClient client = new HttpClient();

        /*
         * The main method - it runs the console application.
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/06/2022
         */
        public static void Main()
        {
            //First, we intialize the SelfLinks array.
            //This method returns null, because we intialize SelfLinks.
            GetSelfLinks("CowCollection.postman_collection.json");

            //Then, we make HTTP requests to get the respective JSON files from each book.
            //We then add the resulting JSON Data to the Contents List<string>.
            GetJsonContents();

            //Then, we deserialize the data.
            //Meaning, we turn the JSON into C# objects.
            DeserializeJson();


            Console.WriteLine("Welcome to Lab 7 - JSON & Postman!");
            Console.WriteLine("The JSON data was automatically deserialized into C# objects.");
            while (true)
            {
                launchMethod(Menu());
            }

            /*string jsonFilePath = Path.Combine(DefaultJsonPath, "AFieldGuidetoCows.json");
            var data = File.ReadAllText(jsonFilePath);
            DeserializeJson(data);*/
        }




        /*  
         * -------------------------------------------------------------------
         * 	            
         * 	            Menu Methods
         * 	            
         * -------------------------------------------------------------------
         */
        /*
         * This method acts as the menu. 
         * Lists the available applications and inputs.
         * Validates user input.
         * 
         * Date Created: 03/21/2022
         * Last Modified: 04/07/2022
         * @return int menuChoice
         */
        private static int Menu()
        {
            string? input = "";
            Console.WriteLine("\nPlease type in the number beside the action you would like to perform.");

            //Continues asking the user for the right input.
            do
            {
                Console.WriteLine("Please type the number next to the option to proceed:");
                Console.WriteLine("1. See Json Data");
                Console.WriteLine("2. See Book Objects");
                Console.WriteLine("3. Create a New Book");
                Console.WriteLine("4. Import All Book Objects");
                Console.WriteLine("5. Delete a Book");
                Console.WriteLine("6. Export All Book Objects");
                Console.WriteLine("7. Exit the Application");
                try
                {
                    input = Console.ReadLine();

                    int menuChoice = Int32.Parse(input.Trim());

                    if (menuChoice >= 1 && menuChoice <= 7)
                    {
                        return menuChoice;
                    }
                    else
                    {
                        Console.WriteLine($"\nOops! \"{input}\" is not an option.");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine($"\nOops! \"{input}\" isn't a number.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine($"\nOops! \"{input}\" is not an option.");
                }
                catch (ArgumentNullException)
                {
                    //Repeat the top message again, asking the user to input a number.
                }
            }
            while (true);
        }
        //end menu()


        /*
         * This method launches a method based on 
         * user input in the menu() method.
         * 
         * Date Created: 03/21/2022
         * Last Modified: 04/07/2022
         */
        private static void launchMethod(int menuChoice)
        {

            //Throws an error saying "The handle in invalid" in Visual Studio Code.
            //Commented out to add support for Visual Studio Code's Debug Console.
            //Console.Clear();

            switch (menuChoice)
            {
                //See Json Data
                case 1:
                    SeeJsonData();
                    break;


                //See Book Objects
                case 2:
                    SeeBookObjects();
                    break;


                //Create a new Book
                case 3:
                    MakeABook();
                    break;


                //Import All Book Objects
                case 4:
                    ImportBookObjects();
                    break;


                //Delete a Book
                case 5:
                    DeleteBook();
                    return;


                //Exit the Application
                case 6:
                    ExportBookObjects();
                    break;

                case 7:
                    Exit();
                    break;

                default:
                    Exit();
                    break;
            }

            Console.WriteLine("\n----- End of Method -----\n\n");
        }
        //end launchMethod()


        /*
        * -------------------------------------------------------------------
        * 	            
        * 	            Assisting Menu Methods
        * 	            
        * -------------------------------------------------------------------
        */

        /*
         * SeeJsonData prints out the JSON Collection text.
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/06/2022
         */
        private static void SeeJsonData()
        {
            Console.WriteLine(File.ReadAllText(Path.Combine(DefaultJsonPath, "CowCollection.postman_collection.json")));
        }

        /*
         * SeeJsonData prints out all Books and their data in the books List<string>.
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/06/2022
         */
        private static void SeeBookObjects()
        {
            foreach (Book book in books)
            {
                Console.WriteLine(book.ToString());
            }
        }

        /*
         * Make a Book utilizes serialization.
         * But, it utilizes user input.
         * 
         * Once we create a book, 
         * we then write a .json file
         * and add the book to the books list.
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/07/2022
         */
        private static void MakeABook()
        {
            try
            {
                //Format:
                /* 0: Id
                 * 1: Title
                 * 2: Description
                 * (another list)
                 * 0: Authors
                 */
                //You might ask, where's SelfLink?
                //Well, we don't expect users to already have their book on Google Books.
                //Plus, if they put a fake website, it won't return JSON and it will break the algorithm.
                //So, we just don't include it.

                //This is definitely convoluted.
                //A List of a List of strings.
                //We do this because one of our inputs requires a List<string>.
                //Because we can't return 2 List<string>s in 1 method, we do this.
                List<List<string>> inputs = BookCreation();

                //Explained Simply:   ID             Title        Authors    Description
                //Types:              String         String    List<string>   String
                Book book = new Book(inputs[0][0], inputs[0][1], inputs[1], inputs[0][2]);
                books.Add(book);

                var serializedBook = JsonConvert.SerializeObject(book);
                Console.WriteLine("Serialized Book Contents: " + serializedBook);
                Contents.Add(serializedBook);
                File.WriteAllText(DefaultJsonPath + inputs[0][1] + ".json", serializedBook);
            }
            catch (Exception)
            {
                Console.WriteLine("Oops! An error was encountered!\nReturning to the Main Menu.");
            }
        }

        /*
         * ImportBookObjects searches the directory for json files.
         * It imports all files except the collection (it has automatically been imported.)
         * 
         * Date Created: 04/07/2022
         * Last Modified: 04/07/2022
         */
        private static void ImportBookObjects()
        {
            try
            {
                string[] allJsonFiles = Directory.GetFiles(DefaultJsonPath);

                foreach (string file in allJsonFiles)
                {
                    var data = File.ReadAllText(file);
                    var book = JsonConvert.DeserializeObject<Book>(data);


                    if (!String.IsNullOrEmpty(book.Id) && !String.IsNullOrEmpty(book.Title))
                    {
                        //books.Contains() does a poor job of comparing books.
                        //Importing multiple times will import the same book over and over again.
                        //We need to do some manual comparing.

                        //Old code.
                        /*
                        books.Add(book);
                        Console.WriteLine(book.ToString() + "\n---------------------------\nSuccessfully added " + book.Title + ".\n---------------------------\n");
                        */

                        //The Manual Comparison Algorithm.
                        Boolean duplicate = false;
                        foreach (var item in books)
                        {
                            if (item.Title == book.Title && item.Id == book.Id)
                            {
                                duplicate = true;
                                Console.WriteLine(book.Title + " was already imported, so we didn't add it to the list.");
                                break;
                            }
                        }
                        if (!duplicate)
                        {
                            books.Add(book);
                            Console.WriteLine(book.ToString() + "\n---------------------------\nSuccessfully added " + book.Title + ".\n---------------------------\n");
                        }
                    }
                    else
                    {
                        //We don't add the book since a book without an ID or title is not really a book.
                        //No need to notify the user.
                    }
                }

                Console.WriteLine("All JSON files found were imported!");
            }
            catch (Exception)
            {
                Console.WriteLine("Oops! An error was encountered!\nReturning to the Main Menu.");
            }
        }

        /*
         * DeleteBook deletes a book from the List<Book>.
         * The change is only temporary and doesn't affect any JSON files.
         * 
         * Date Created: 04/07/2022
         * Last Modified: 04/07/2022
         */
        private static void DeleteBook()
        {
            foreach (var book in books)
            {
                Console.WriteLine(book.ToString());
            }

            Console.WriteLine("\nPlease enter the ID of the book you want to delete.");
            string compareID = Console.ReadLine();

            foreach (var book in books)
            {
                if (book.Id.Trim() == compareID.Trim())
                {
                    books.Remove(book);
                    Console.WriteLine("The book \"" + book.Title + "\" was successfully deleted.");
                    return;
                }
            }

            Console.WriteLine("We couldn't find a book to delete.");
            return;
        }


        /*
         * ExportBookObjects serializes all Books in the List<Book> 
         * and turns them into json files.
         * 
         * Date Created: 04/07/2022
         * Last Modified: 04/07/2022
         * @param Boolean singularExport
         * @param int index
         */
        private static void ExportBookObjects(Boolean singularExport = false, int index = 0)
        {
            try
            {
                if (singularExport)
                {
                    var test = JsonConvert.SerializeObject(books[index], new JsonSerializerSettings { Formatting = Formatting.Indented });

                    Console.WriteLine(test.ToString());
                    File.WriteAllText(DefaultJsonPath + books[index].Title + ".json", test.ToString());
                }
                else
                {
                    for (int i = 0; i < books.Count; i++)
                    {
                        var test = JsonConvert.SerializeObject(books[i], new JsonSerializerSettings { Formatting = Formatting.Indented });

                        Console.WriteLine(test.ToString());
                        File.WriteAllText(DefaultJsonPath + books[i].Title + ".json", test.ToString());
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Oops! An error was encountered!\nReturning to the Main Menu.");
            }
        }


        /*
         * Exit exits the application.
         * 
         * Date Created: 04/07/2022
         * Last Modified: 04/07/2022
         */
        private static void Exit()
        {
            Console.WriteLine("Thank you for using this JSON/POSTMAN Application!");
            System.Environment.Exit(0);
        }








        /*
        * -------------------------------------------------------------------
        * 	            
        * 	            Other Methods - Deserialization
        * 	            
        * -------------------------------------------------------------------
        */

        /*
         * This method gets the self links of all books in the (postman) JSON collection.
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/07/2022
         * 
         * @param string fileName
         * @param Boolean singularLink
         * @param int index
         * @return fourthResult.Raw
         */
        private static string? GetSelfLinks(string fileName, Boolean singularLink = false, int index = 0)
        {
            //jsonPath points to the jsonfile we need in the JsonData directory.
            //In this case, it's "CowCollection.postman_collection.json".
            string jsonFilePath = Path.Combine(DefaultJsonPath, fileName);
            var data = File.ReadAllText(jsonFilePath);
            //Console.WriteLine(data + "\n--------------------------------------------\n");


            JsonModel json = JsonConvert.DeserializeObject<JsonModel>(data);


            if (singularLink)
            {
                Console.WriteLine("Raw: \"" + json.Raw + "\"");
                SelfLinks.Add(json.Raw);
            }
            else
            {
                //otherwise, we deserialize all items and add the self links to the SelfLinks List<string>.
                for (int i = 0; i < json.Item.Count; i++)
                {
                    var newData = json.Item[i];
                    Console.WriteLine("Data: " + json.Item[i].ToString());

                    var test = JsonConvert.DeserializeObject<JsonModel>(json.Item[i].ToString());
                    Console.WriteLine("Test: " + test.Request.ToString());

                    var test2 = JsonConvert.DeserializeObject<JsonModel>(test.Request.ToString());
                    Console.WriteLine("Test 2: " + test2.Url.ToString());

                    var test3 = JsonConvert.DeserializeObject<JsonModel>(test2.Url.ToString());
                    Console.WriteLine("Test 3: " + test3.Raw);

                    Console.WriteLine("Raw: \"" + test3.Raw + "\"");
                    SelfLinks.Add(test3.Raw);
                }
                //Since we already added the links to the list, we just return null.
                //This lets the algorithm know that we intialized SelfLinks.
                return null;
            }

            return json.ToString();
        }

        /*
         * This method makes a HTTP request for a specified book. 
         * This HTTP request will return a JSON file. 
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/07/2022
         * @param Boolean singleContent
         * @param int index
         * @return JsonContents
         */
        private static string? GetJsonContents(Boolean singleContent = false, int index = 0)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            //client.DefaultRequestHeaders.Add()

            try
            {
                if (singleContent)
                {
                    //client.GetStringSync gets the JSON file from the link by making a connection to the webpage.
                    var JsonContents = client.GetStringAsync(SelfLinks[index]);

                    //JsonContents.Result is the resulting JSON file returned by the server.
                    return JsonContents.Result;
                }
                else
                {
                    for (int i = 0; i < SelfLinks.Count; i++)
                    {
                        //client.GetStringSync gets the JSON file from the link by making a connection to the webpage.
                        var JsonContents = client.GetStringAsync(SelfLinks[i]);

                        //JsonContents.Result is the resulting JSON file returned by the server.
                        Contents.Add(JsonContents.Result);
                    }
                    //We return null to let the algorithm know that we initialized Contents.
                    return null;
                }

            }
            catch (AggregateException)
            {
                //Running an HTTP Request with a bad (or no) internet connection may result in
                //an AggregateException. So, we catch the exception and inform the user.

                Console.WriteLine("Looks like the application couldn't connect to the Google Books API.\nPlease check your internet connection!");
            }

            return null;
        }


        /*
         * This method deserializes JSON and 
         * converts a JSON object into C#.
         * 
         * This method also addresses a medium-size obstacle.
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/07/2022
         * @param Boolean singleDeserialization
         * @param int index
         */
        private static void DeserializeJson(Boolean singleDeserialization = false, int index = 0)
        {
            //There's a problem. Flashbacks to Lab 4.
            /* -------------------------------------------------
             *  ****************  The Problem  ****************
             * -------------------------------------------------
             * As it stands, JSONSerializer.Deserialize doesn't make a book in one go.
             * That's because Title, Authors, and Description are inside a VolumeInfo array.
             * 
             * What does THAT mean?!
             * The compiler looks if there's a VolumeInfo array in Books.
             * If there isn't, the compiler goes right to the next value. 
             * The compiler doesn't check what's inside the array for values.
             * 
             * What do we DO now?!
             * If you think about it, the contents inside the JSON file are in an array.
             * Items in that first array are compared to Book.
             * Because the Book's ID and SelfLink were in that first array, the compiler saw them
             * and correctly Deserialized them into the Book instance.
             * 
             * The other attributes are inside VolumeInfo, an array inside an array.
             * >>> So, We NEED to DELVE into VolumeInfo if we want Title, Authors, and Description. <<<
             * 
             * 
             * In other words, VolumeInfo is like a string array (string[]) inside the first array.
             * SO, we go inside the VolumeInfo class and Deserialize everything inside.
             * There are even arrays inside the VolumeInfo class, 
             * but this lab assignment doesn't focus on those.
             * 
             * We assign the first deserialization to a var firstResult,
             * and assign the second deserialization to a var secondResult.
             * 
             * We can then extract the resulting values from both deserializations
             * and make a book fitting our class from them. 
             * --------------------------------------------------
             *  **************** Problem Solved ****************
             * --------------------------------------------------
             */
            if (singleDeserialization)
            {
                //Assigns the Book's ID and SelfLink. 
                //Skips past Title, Authors, and Description.
                var firstResult = JsonConvert.DeserializeObject<Book>(Contents[index]);


                //Take the values we extracted from firstResult and secondResult and make a new Book object out of them.
                //It seems strange having to make 2 books to make 1 full book. But, it works!
                books.Add(new Book(firstResult.Id, firstResult.Title, firstResult.Authors, firstResult.Description, firstResult.SelfLink));
                Console.WriteLine("Finished Product: " + books[books.Count - 1].ToString());
            }
            else
            {
                for (int i = 0; i < Contents.Count; i++)
                {
                    //Assigns the Book's ID and SelfLink. 
                    //Skips past Title, Authors, and Description.
                    var firstResult = JsonConvert.DeserializeObject<Book>(Contents[i]);

                    Console.WriteLine("Result of First Result: " + firstResult.ToString());
                    //Console.WriteLine("Content: " + jsonContents[i]);

                    //This gets the VolumeInfo array by initializing the VolumeInfo field in the JsonModel VolumeInfo. 
                    JsonModel test = JsonConvert.DeserializeObject<JsonModel>(Contents[i]);

                    //Assigns the Book's Title, Authors, and Description.
                    var secondResult = JsonConvert.DeserializeObject<Book>(test.VolumeInfo.ToString());
                    Console.WriteLine("Result of Second Result: " + firstResult.ToString());
                    //JsonModel VolumeInfo = JsonSerializer.Deserialize<JsonModel>(jsonContents[0], new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    //Take the values we extracted from firstResult and secondResult and make a new Book object out of them.
                    //It seems strange having to make 2 books to make 1 full book. But, it works!
                    books.Add(new Book(firstResult.Id, secondResult.Title, secondResult.Authors, secondResult.Description, firstResult.SelfLink));
                    Console.WriteLine("Finished Product: " + books[books.Count - 1].ToString());
                }
            }
        }


        /*
        * -------------------------------------------------------------------
        * 	            
        * 	            Other Methods - Serialization
        * 	            
        * -------------------------------------------------------------------
        */

        /*
         * Guides the user through a Book creation process.
         * 
         * Date Created: 04/06/2022
         * Last Modified: 04/07/2022
         * 
         * @return List<List<string>> bigList
         */
        private static List<List<string>> BookCreation()
        {
            //For reference, this is what we're dealing with.
            //Explained Simply:      ID             Title       Authors    Description
            //Types:                 String         String    List<string>   String
            //Book book = new Book(inputs[0][0], inputs[0][1], inputs[1], inputs[0][2]);


            List<List<string>> bigList = new();


            List<string> bookAttributes = new();


            //-----BOOK ID-----
            //Now, we create a random ID.
            StringBuilder sb = new();
            Random random = new Random();

            //We randomly generate an ID of Length 12.
            //All the other IDs are of Length 12.
            for (int i = 0; i < 12; i++)
            {
                //Randomly choose 0, 1, or 2.
                int randomNum = random.Next(3);

                if (randomNum == 0)
                {
                    //Append a number from 0 - 9.
                    sb.Append(random.Next(10));
                }
                else if (randomNum == 1)
                {
                    //Thanks to https://www.codegrepper.com/code-examples/csharp/how+to+generate+random+letters+in+C%23
                    //ASCII character codes 65-90 represent upper-case letters. 
                    int ascii_index = random.Next(65, 91);//ASCII character codes 65-90
                    sb.Append(Convert.ToChar(ascii_index)); //produces any char A-Z
                }
                else
                {
                    int ascii_index2 = random.Next(97, 123); //ASCII character codes 97 - 123 represent lower-case letters.
                    sb.Append(Convert.ToChar(ascii_index2)); //produces any char a-z
                }
            }
            //the 0th index of the 0th list in bigList is assigned the ID. 
            bookAttributes.Add(sb.ToString());


            //-----BOOK TITLE-----
            string? input = "";
            do
            {
                Console.WriteLine("Please input the Book's Title.\n(Type \"Stop\" to Return to the Main Menu)");
                input = Console.ReadLine();
                if (String.IsNullOrEmpty(input))
                {
                    Console.WriteLine("The input was blank!\nPlease try again.");
                }
                else if (input == "Stop" || input == "stop" || input == "\"Stop\"" || input == "\"stop\"")
                {
                    //We return null if the user quits out.
                    //Don't worry - the algorithm is prepared for this scenario.
                    return null;
                }
            } while (String.IsNullOrEmpty(input));

            Console.WriteLine("\n--------------------\nYou inputted: " + $"\"{input}\"\n--------------------\n\n");
            //the 1st index of the 0th list in bigList is assigned the Title. 
            bookAttributes.Add(input);


            //-----BOOK DESCRIPTION-----
            input = "";
            do
            {
                Console.WriteLine("Please input a Description of the Book.\n(Type \"Stop\" to Return to the Main Menu)");
                input = Console.ReadLine();
                if (String.IsNullOrEmpty(input))
                {
                    Console.WriteLine("The input was blank!\nPlease try again.");
                }
                else if (input == "Stop" || input == "stop" || input == "\"Stop\"" || input == "\"stop\"")
                {
                    //We return null if the user quits out.
                    //Don't worry - the algorithm is prepared for this scenario.
                    return null;
                }
            } while (String.IsNullOrEmpty(input));

            Console.WriteLine("\n--------------------\nYou inputted: " + $"\"{input}\"\n--------------------\n\n");
            //the 2st index of the 0th list in bigList is assigned the Description. 
            bookAttributes.Add(input);



            //-----BOOK AUTHORS-----
            List<string> authors = new();
            input = "";
            do
            {
                Console.WriteLine("Please input the names of the authors of the Book.\n(Type \"Done\" to Create the Book!)\n(Type \"Stop\" to Return to the Main Menu.)");
                input = Console.ReadLine();

                if (String.IsNullOrEmpty(input))
                {
                    Console.WriteLine("The input was blank!\nPlease try again.");
                }
                else if (input == "Stop" || input == "stop" || input == "\"Stop\"" || input == "\"stop\"")
                {
                    //We return null if the user quits out.
                    //Don't worry - the algorithm is prepared for this scenario.
                    return null;
                }
                else if (input == "Done" || input == "done" || input == "\"Done\"" || input == "\"done\"")
                {
                    bigList.Add(bookAttributes);
                    bigList.Add(authors);
                    return bigList;
                }
                else
                {
                    authors.Add(input);
                    Console.WriteLine($"\n--------------------\n{input} was added as an author!\n--------------------\n");
                }
            } while (true);
            //loop forever, until the user types "Done".
        }







    }

    /*
     * The JsonModel class's sole purpose
     * is to assist in retrieving json values.
     * 
     * It contains all the fields necessary to finish the tasks
     * of lab 7.
     * 
     * Date Created: 04/06/2022
     * Last Modified: 04/07/2022
     */
    public class JsonModel
    {
        public object VolumeInfo { get; set; }
        //VolumeInfo will encapsulate the array in the first array.

        public List<object> Item { get; set; }
        //Item will help us determine how many Books there are in the array.
        //This will allows the program to be more dynamic in terms of number of books.

        public object Request { get; set; }
        //Request is a JSON array. Inside it, is raw.

        public object Url { get; set; }
        //Url is in Request

        public string Raw { get; set; }

        //Raw is another definition of SelfLink, but is worded as "raw" in Json.
        //Quite the pain to navigate through 3 arrays (item, request, url), in order to retrieve this.
    }
}