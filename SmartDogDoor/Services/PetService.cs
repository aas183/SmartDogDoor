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

                String sql = "SELECT PetID, PetName, PetImage, InOut FROM PetInfoTable";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pet pet = new Pet();
                            pet.ID = reader.GetString(0);
                            pet.Name = reader.GetString(1);
                            pet.Image = reader.GetString(2);
                            string inOut = reader.GetString(4);
                            if(String.Compare(inOut,"0") == 0)
                            {
                                pet.InOut = "Out";
                            }
                            else
                            {
                                pet.InOut = "In";
                            }
                            petList.Add(pet);
                            
                            Console.WriteLine("{0} {1} {2} {3}", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
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


