namespace SmartDogDoor.Services;
using System.Net.Http.Json;
using System;
using Microsoft.Data.SqlClient;
using System.Text;

//Class for accessing outside data from pet server
public class PetService
{ 
    public PetService()
    {
    }

    //Lists for entries from database tables
    List<Pet> petList = new ();
    List<PetActivity> petActivityList = new();
    List<Lock> lockList = new();

    //Function to get data from Pet Information Database Table
    public async Task<List<Pet>> GetPets()
    {
        //Note: Need to make more async
        try
        {
            petList.Clear();//clear current pet list

            //Create connection string to database
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "pet-server.database.windows.net";
            builder.UserID = "drewshetler";
            builder.Password = "DT01-Dog-D00r";
            builder.InitialCatalog = "Pet-Database";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))//use connection object
            {
                Console.WriteLine("\nQuery data:");
                Console.WriteLine("=========================================\n");

                String sql = "SELECT Id, Name, Image, InOut FROM Pet_Information_Table";//selection from database

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    await connection.OpenAsync();//connect to database

                    using (SqlDataReader reader = command.ExecuteReader())//use reader object 
                    {
                        while (reader.Read())//read until NULL entry in database
                        {
                            //Create new Pet Object
                            Pet pet = new Pet();
                            //Console.WriteLine(reader.GetString(0));
                            
                            //Store variables from database in pet object members
                            pet.Id = Convert.ToString(reader.GetInt64(0));
                            pet.Name = reader.GetString(1);
                            pet.Image = reader.GetString(2);
                            bool inOut = reader.GetBoolean(3);
                      
                            //set inOut memeber variables
                            if(inOut)
                            {
                                pet.InOut = "In";
                                pet.InOutColor = Color.FromRgba("#008450");
                            }
                            else
                            {
                                pet.InOut = "Out";
                                pet.InOutColor = Color.FromRgba("#B81D13");
                            }
                            petList.Add(pet);//add pet to list of pets
                            
                            Console.WriteLine("{0} {1} {2} {3}", pet.Id, pet.Name, pet.Image, pet.InOut);
                        }
                    }
                }
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
        
        //Console.ReadLine();
        return petList;
        
    }

    /*
    public async void addPet()
    {
        //connect to database

        //read number of entries in pet information database table

        //add a new row to database table and make the pet name Pet<numofPets+1> and make in

    }
    */

    /*
    public async void deletePet(string id)
    {
        //connect to database

        //read number of entries in pet information database table

        //add find row with passed id in pet information database table and delete pet
        //find enetires in pet activty database table with passed id and delete them
    }
    */

    /*
   public async void changePetName(string id, string name)
   {
       //connect to database

       //find entry in pet information database table with passed id

       //change found entry with passed id's name to passed name
   }
   */

    /*
    public async string addPetImage(image img)
    {
        //connect to Blob storage

        //add upload passed image into blob storage

        //return URL
    }
    */

    /*
    public async string addPetImageDatabase(string URL, string Id)
    {
        //connect to database

        //find entry in pet information database table with passed id

        //change found entry with passed id's image link to passed URL
    }
    */

    /*
    public async void deletePetImages(string ID)
    {
        //connect to database and Blob storage

        //search pet information database table and pet activity database table for all entries for passed ID and get all 
        //entries image URLs

        //Using URL for images go into blob storage and delete all images with the receieved URLS
    }
    */

    /*
    public async string deletePetImage(string URL)
    {
        //connect to Blob storage

        //delete image entry with passed URL

        //return old image URL
    }
    */

    //Not Completed
    //Function for getting entries from Pet Activity database table.
    public async Task<List<PetActivity>> GetPetActvities()
    {
        try
        {
            //Pet pet;


            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "pet-server.database.windows.net";
            builder.UserID = "drewshetler";
            builder.Password = "DT01-Dog-D00r";
            builder.InitialCatalog = "Pet-Database";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                String sql = "SELECT Id, Name, Image, InOut FROM Pet_Info_Table";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PetActivity pet = new PetActivity();
                            //Console.WriteLine(reader.GetString(0));
                            pet.Id = Convert.ToString(reader.GetInt64(0));
                            //pet.Name = reader.GetString(1);
                            pet.Image = reader.GetString(2);
                            bool inOut = reader.GetBoolean(3);
                            if (inOut)
                            {
                                pet.InOut = "In";
                            }
                            else
                            {
                                pet.InOut = "Out";
                            }
                            petActivityList.Add(pet);

                            Console.WriteLine("{0} {1} {2}", pet.Id, pet.Image, pet.InOut);
                        }
                    }
                }
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }

        //Console.ReadLine();
        return petActivityList;

    }

    //Not Completed
    //Function for getting entries from locking restriction database table.
    public async Task<List<Lock>> GetLocks()
    {
        try
        {
            //Pet pet;


            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "pet-server.database.windows.net";
            builder.UserID = "drewshetler";
            builder.Password = "DT01-Dog-D00r";
            builder.InitialCatalog = "Pet-Database";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("\nQuery data:");
                Console.WriteLine("=========================================\n");

                String sql = "SELECT Id, Name, Image, InOut FROM Pet_Info_Table";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lock pet = new Lock();
                            //Console.WriteLine(reader.GetString(0));
                            /*
                            pet.Id = Convert.ToString(reader.GetInt64(0));
                            //pet.Name = reader.GetString(1);
                            pet.Image = reader.GetString(2);
                            bool inOut = reader.GetBoolean(3);
                            if (inOut)
                            {
                                pet.InOut = "In";
                            }
                            else
                            {
                                pet.InOut = "Out";
                            }
                            petActivityList.Add(pet);

                            Console.WriteLine("{0} {1} {2}", pet.Id, pet.Image, pet.InOut);
                            */
                        }
                    }
                }
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }

        //Console.ReadLine();
        return lockList;

    }

    /*
    public async void addLock(string timeStart, string timeStop, bool lockUnlock)
    {
        //connect to database

        //add entry to locking restriction database table with passed parameters

    }
    */

    /*
    public async void deleteLock(Lock lock)
    {
        //connect to database

        //delete entry of locking restriction database table with has entires same as the members of passed Lock object
    }
    */


    /*
    if(petList?.Count >  0) 
        return petList;

    var url = "";

    var response = await httpClient.GetAsync(url);
    //use asynce for webapi calls
    if(response.IsSuccessStatusCode)
    {
        petList  = await response.Content.ReadFromJsonAsync<List<Pet>>();
    }

    return petList;
    */
}


