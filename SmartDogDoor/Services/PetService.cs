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
    if(petList?.Count >  0) 
        return petList;

    var url = "https://montemagno.com/monkeys.json";

    var response = await httpClient.GetAsync(url);
    //use asynce for webapi calls
    if(response.IsSuccessStatusCode)
    {
        petList  = await response.Content.ReadFromJsonAsync<List<Pet>>();
    }

    return petList;
    */
}


