namespace SmartDogDoor.Services;
using System.Net.Http.Json;
using System;
using Microsoft.Data.SqlClient;
using System.Text;


public class PetService
{ 
    public PetService()
    {
    }

    List<Pet> petList = new ();
    List<PetActivity> petActivityList = new();
    List<Lock> lockList = new();
    public async Task<List<Pet>> GetPets()
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
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pet pet = new Pet();
                            //Console.WriteLine(reader.GetString(0));
                            pet.Id = Convert.ToString(reader.GetInt64(0));
                            pet.Name = reader.GetString(1);
                            pet.Image = reader.GetString(2);
                            bool inOut = reader.GetBoolean(3);
                            if(inOut)
                            {
                                pet.InOut = "In";
                            }
                            else
                            {
                                pet.InOut = "Out";
                            }
                            petList.Add(pet);
                            
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
                    connection.Open();
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
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                String sql = "SELECT Id, Name, Image, InOut FROM Pet_Info_Table";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
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


