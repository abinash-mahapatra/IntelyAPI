using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IntelyAPI.Logic
{
    public class TagDocumentFilter :IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag
                {
                    Name= "Generate Token",
                    Description= "Generate a Token to access all methods."
                },
                new OpenApiTag
                {
                    Name= "Identity Management",
                    Description = "The Identity Management API routes authenticate endpoint access for a user wanting to make requests to intely API."
                },
                new OpenApiTag
                {
                    Name= "Apps",
                    Description= "The App API allows you to retrive a list of authenticated vendors and applications from the intely Platform. APP is the system that is going to be accessed (e.g. Cerner,Epic,etc.) and instance is the specific location."
                },
                new OpenApiTag
                {
                    Name= "EMR",
                    Description= "Routes that deal with sending and reciving data from EMRs.",
                },
                //new OpenApiTag
                //{
                //    Name= "EMR Events",
                //    Description= "Routes that deal with sending data to EMRs based on events."
                //}

            };
            
        }
    }
}
