
using IntelyAPI;
using IntelyAPI.Logic;
using IntelyAPI.MiddleWare;
using IntelyAPI.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SecurityRequirementsOperationFilter = IntelyAPI.MiddleWare.SecurityRequirementsOperationFilter;

var builder = WebApplication.CreateBuilder(args);


var ApiConnectionString = builder.Configuration.GetConnectionString("APIURL");
var apiURL = builder.Configuration.GetConnectionString("URL");
var token = string.Empty;
int exptime = 0;
HttpClient client = new HttpClient();
var Authtoken = string.Empty;
var tokenForAccessAllMethods = "";
var user = builder.Configuration["user"];
var password = builder.Configuration["password"];
string isValidUrl = builder.Configuration["ValidUrl"].ToString();
LogError logs = new LogError();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.DocumentFilter<TagDocumentFilter>();
    option.EnableAnnotations();
    option.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues}");
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Intely API", Version = "v1", Description = @"The Intely API provides a way to programmatically access and build on the Intely Platform. Intely provides infrastructure to connect data and automate interoperability workflows for vendors, applications and provider organizations. <br> <br>
                                                                                                                                                            Intely has provided a variety of API endpoints to call to authenticate and connect to applications,execute pre - built integrations and data conversions,and use our form building modules.Many features need to be pre - configured in our platform,you can then select the APIs for your use case and implement quite easily.<br>" });
    option.OperationFilter<SecurityRequirementsOperationFilter>();
    option.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Basic",
        Description = "Input your username and password to access this API",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basicAuth"
                }
            },
            new[]  {"DemoSwaggerDifferentAuthScheme"}
        }
    });

    option.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter Token From Intely",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="bearerAuth"
                }
            },
            new[] { "DemoSwaggerDifferentAuthScheme"}
        }
    });
});
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddAuthentication(opt =>
{}).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);


//builder.Services.AddAuthentication("BasicAuthentication")
//       .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);


builder.Services.AddAuthentication().AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"], 
        ValidAudience = builder.Configuration["JWT:ValidAudiance"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
    };
});


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DocumentTitle = "Intely API";
    c.HeadContent = @"        <link rel='stylesheet' href='https://dev2021.dsg-us.com/IntelyAPI/swagger/custom.css' /> ";
    //c.InjectJavascript("/wwwroot/assets/script.js");
    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "My API V1");
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseAuthentication();
app.UseAuthorization();


app.MapPost("/Login", [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.AuthenticationScheme)] async (HttpRequest Request) =>
{

    string rResp = string.Empty;
    AuthenticationHelper authenticationHelper = new AuthenticationHelper(builder.Configuration);
    string authHeader = Request.Headers["Authorization"];
    if (authHeader != null && authHeader.StartsWith("Basic"))
    {

        string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
        Encoding encoding = Encoding.GetEncoding("iso-8859-1");
        string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
        int separatorIndex = usernamePassword.IndexOf(':');
        string username = usernamePassword.Substring(0, separatorIndex);
        string password = usernamePassword.Substring(separatorIndex + 1);

        var checkLogin = new userValidate()
        {
            UserName = username,
            UserPassKey = password,
        };
        var content = new StringContent(JsonConvert.SerializeObject(checkLogin), System.Text.Encoding.UTF8, "application/json");
        string url = isValidUrl;
        var response = await client.PostAsync(url, content);
        rResp = await response.Content.ReadAsStringAsync();
        bool isValid = JsonConvert.DeserializeObject<bool>(rResp);
        if (isValid)
        {

            tokenForAccessAllMethods = authenticationHelper.Login();
        }
    };
    if (!string.IsNullOrEmpty(tokenForAccessAllMethods))
        return tokenForAccessAllMethods;
    else
        return "Unauthorized";

}).WithTags("Generate Token").WithMetadata(new SwaggerOperationAttribute("Generate a token to access all the methods."));

//app.Use(async (context, next) =>
//{
//    if (context.Request.Headers.ContainsKey("Authorization") &&
//        context.Request.Headers["Authorization"][0].StartsWith("Bearer "))
//    {
//             Authtoken = context.Request.Headers["Authorization"][0]
//            .Substring("Bearer ".Length);
//    }

//    await next.Invoke();
//});


app.MapGet("/jwks",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async () =>
{
    string rResp = string.Empty;
    try
    {
        string _apiUrl = ApiConnectionString + "jwks";
        var responce = await client.GetAsync(_apiUrl);
        if (responce.IsSuccessStatusCode)
        {
            rResp = await responce.Content.ReadAsStringAsync();
            //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

        }
        JwksModel data = JsonConvert.DeserializeObject<JwksModel>(rResp);
        return data;
    }
    catch (Exception ex)
    {
        throw ex;
    }
}).WithTags("Identity Management").WithMetadata(new SwaggerOperationAttribute("Returns the identity Server's public key set in the JWKS format."));

app.MapPost("/user/login_token",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async () =>
{
    string rResp = string.Empty;
    //string authDetails = user + ":" + password;
    try
    {
        var byteArray = Encoding.ASCII.GetBytes($"{user}:{password}");
        string json = string.Empty;
        var stringContent = new StringContent(json);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        string url = ApiConnectionString + "noauth/user/login/token";
        var response = await client.PostAsync(url, stringContent);

        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();

        }
        //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

        //data = JsonConvert.DeserializeObject(data, typeof(object));
        TokenModel tokenModel = JsonConvert.DeserializeObject<TokenModel>(rResp);
        token = tokenModel.token.ToString();
        exptime = (int)tokenModel.tokenPayload.exp;
        return tokenModel;
    }
    catch (Exception ex)
    {
        throw ex;
    }
}).WithTags("Identity Management").WithMetadata(new SwaggerOperationAttribute("Gets a user token from a personal access token."));

app.MapGet("/RefreshToken",
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async () =>
{

    string rResp = string.Empty;
    tokenPayload tokenData = null;
    //DateTime initialexpiry = (DateTime.UtcNow).AddMilliseconds(double.Parse(exptime.ToString()));
    //DateTime dtnow = DateTime.UtcNow;
    //int remainingTime = DateTime.Compare(initialexpiry, dtnow);

    try
    {
        //if (remainingTime < 0)
        //{
        //    var stringContent = new StringContent("");
        //    string url = "http://localhost:7124/" + "user/login_token";
        //    var response = await client.PostAsync(url, stringContent);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        rResp = await response.Content.ReadAsStringAsync();

        //    }
        //    TokenModel tokenModel = JsonConvert.DeserializeObject<TokenModel>(rResp);
        //    token = tokenModel.token.ToString();
        //    string _apiUrl = ApiConnectionString + "currentUser/validate";
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

        //    var responce = await client.GetAsync(_apiUrl);
        //    if (responce.IsSuccessStatusCode)
        //    {
        //        rResp = await responce.Content.ReadAsStringAsync();
        //        //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

        //    }
        //    tokenData = JsonConvert.DeserializeObject<tokenPayload>(rResp);

        //}

        string _apiUrl = ApiConnectionString + "currentUser/validate";

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responce = await client.GetAsync(_apiUrl);
        if (responce.IsSuccessStatusCode)
        {
            rResp = await responce.Content.ReadAsStringAsync();
            //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

        }
        else
        {
            var stringContent = new StringContent("");
            string url = apiURL + "user/login_token";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForAccessAllMethods);
            var response = await client.PostAsync(url, stringContent);
            string currentResp = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                currentResp = await response.Content.ReadAsStringAsync();

            }
            TokenModel tokenModel = JsonConvert.DeserializeObject<TokenModel>(currentResp);
            token = tokenModel.token.ToString();

            string _appUrl = ApiConnectionString + "currentUser/validate";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync(_appUrl);
            if (resp.IsSuccessStatusCode)
            {
                rResp = await responce.Content.ReadAsStringAsync();
                //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

            }
        }
        tokenData = JsonConvert.DeserializeObject<tokenPayload>(rResp);

        return token;
    }
    catch (Exception ex)
    {
        throw ex;
    }

}).WithTags("Identity Management").WithMetadata(new SwaggerOperationAttribute("Validates a user token.")).ExcludeFromDescription();



//app.MapGet("/apps",
//    async (string token, string currentOrganizationId, bool publicc) =>
//    {
//        string rResp = string.Empty;
//        try
//        {
//            string _apiUrl = ApiConnectionString + "apps?public=" + publicc;
//            client.DefaultRequestHeaders.Add("Current-Organization-Id", currentOrganizationId);
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//            var responce = await client.GetAsync(_apiUrl);
//            if (responce.IsSuccessStatusCode)
//            {
//                rResp = await responce.Content.ReadAsStringAsync();
//                //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

//            }
//            AppsModel appsModel = JsonConvert.DeserializeObject<AppsModel>(rResp);
//            return appsModel;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }).WithTags("Apps").WithMetadata(new SwaggerOperationAttribute("Returns a list of all the apps the user has access to"));

//app.MapGet("/app/{appId}",
//    async (string token, string currentOrganizationId, string appId) =>
//    {
//        string rResp = string.Empty;
//        try
//        {
//            string _apiUrl = ApiConnectionString + "app/" + appId + currentOrganizationId;
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//            var responce = await client.GetAsync(_apiUrl);
//            if (responce.IsSuccessStatusCode)
//            {
//                rResp = await responce.Content.ReadAsStringAsync();
//                //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

//            }
//            return rResp;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }).WithTags("Apps").WithMetadata(new SwaggerOperationAttribute("Returns details of a specific app"));

app.MapGet("/app/instances",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async ([SwaggerParameter("The organization id to use")][FromQuery(Name = "Current-Organization-Id")] string currentOrganizationId) =>
    {
        AppInstanceModel instancedata = null;
        string rResp = string.Empty;
        try
        {
            string uri = apiURL + "RefreshToken";

            var content = new StringContent("");

            //string url = apiURL + $"UserValidate?token1={token}";
            //var resp = await client.GetAsync(url);

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForAccessAllMethods);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string _apiUrl = ApiConnectionString + "app/instances";
                var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
                request.Headers.Add("Current-Organization-Id", currentOrganizationId);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var responce = await client.SendAsync(request);
                if (responce.IsSuccessStatusCode)
                {
                    rResp = await responce.Content.ReadAsStringAsync();
                    //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));
                }
                instancedata = JsonConvert.DeserializeObject<AppInstanceModel>(rResp);

            }
            return instancedata;
        }
        catch (Exception ex)
        {
            logs.logsData(ex.Message);
            throw ex;
        }
    }).WithTags("Apps").WithMetadata(new SwaggerOperationAttribute("Returns a list of all the app instances the user has access to"));

app.MapGet("/appUserAccessInstances",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async ([SwaggerParameter("The organization id to use")][FromHeader(Name = "Current-Organization-Id")] string currentOrganizationId, [SwaggerParameter("The ID of the app")] string appId) =>
    {
        AppInstanceModel instancedataApiBased = null;
        string rResp = string.Empty;
        try
        {
            string uri = apiURL + "RefreshToken";
            var content = new StringContent("");

            //string url = apiURL + $"UserValidate?token1={token}";
            //var resp = await client.GetAsync(url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForAccessAllMethods);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string _apiUrl = ApiConnectionString + "app/" + appId + "/instances";
                var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
                request.Headers.Add("Current-Organization-Id", currentOrganizationId);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var responce = await client.SendAsync(request);
                if (responce.IsSuccessStatusCode)
                {
                    rResp = await responce.Content.ReadAsStringAsync();
                    //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

                }
                instancedataApiBased = JsonConvert.DeserializeObject<AppInstanceModel>(rResp);
            }
            return instancedataApiBased;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }).WithTags("Apps").WithMetadata(new SwaggerOperationAttribute("Returns a list of all the app the user has access to for the given app"));

app.MapGet("/appSpecificInstance",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async ([SwaggerParameter("The organization id to use")][FromHeader(Name = "Current-Organization-Id")] string currentOrganizationId, [SwaggerParameter("The ID of the app")] string appId, [SwaggerParameter("The ID of the app instance")] string instanceId) =>
    {
        AppInstanceModel appInstancedata = null;
        string rResp = string.Empty;
        try
        {
            string uri = apiURL + "RefreshToken";
            var content = new StringContent("");

            //string url = apiURL + $"UserValidate?token1={token}";
            //var resp = await client.GetAsync(url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForAccessAllMethods);
            var resp = await client.GetAsync(uri);
            if (resp.IsSuccessStatusCode)
            {
                string _apiUrl = ApiConnectionString + "app/" + appId + "/instance/" + instanceId;
                var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);

                request.Headers.Add("Current-Organization-Id", currentOrganizationId);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var responce = await client.SendAsync(request);
                if (responce.IsSuccessStatusCode)
                {
                    rResp = await responce.Content.ReadAsStringAsync();
                    //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

                }
                appInstancedata = JsonConvert.DeserializeObject<AppInstanceModel>(rResp);
            }

            return appInstancedata;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }).WithTags("Apps").WithMetadata(new SwaggerOperationAttribute("Returns details of a specific app instance"));

//app.MapPost("/appResourceInstance",
//    async (string currentOrganizationId, string appId, string instanceId, string resourceId, string actionId) =>
//    {
//        string rResp = string.Empty;
//        try
//        {

//            string json = string.Empty;
//            var stringContent = new StringContent(json);
//            string _apiUrl = ApiConnectionString + "app/" + appId + "/instance" + instanceId+ "request/"+ resourceId + actionId + currentOrganizationId  ;
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//            var responce = await client.PostAsync(_apiUrl, stringContent);
//            if (responce.IsSuccessStatusCode)
//            {
//                rResp = await responce.Content.ReadAsStringAsync();
//                //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

//            }
//            return rResp;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }).WithTags("Apps").WithMetadata(new SwaggerOperationAttribute("Makes a request to an app resource"));

app.MapGet("/EmrGetAllPatient",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async ([FromHeader(Name = "Current-Organization-Id")] string currentOrganizationId, [FromHeader(Name = "App-Id")] string appId, [FromHeader(Name = "Instance-Id")] string instanceId, string name) =>
    {
        EMRPatientModel emrPatientData = null;
        string rResp = string.Empty;
        try
        {

            //client.DefaultRequestHeaders.Add("Current-Organization-Id", currentOrganizationId);
            //client.DefaultRequestHeaders.Add("App-Id", appId);
            //client.DefaultRequestHeaders.Add("Instance-Id", instanceId);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string uri = apiURL + "RefreshToken";
            var content = new StringContent("");

            //string url = apiURL + $"UserValidate?token1={token}";
            //var resp = await client.GetAsync(url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForAccessAllMethods);
            var resp = await client.GetAsync(uri);
            if (resp.IsSuccessStatusCode)
            {
                string _apiUrl = ApiConnectionString + "emr/patient?name=" + name;
                var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
                request.Headers.Add("Current-Organization-Id", currentOrganizationId);
                request.Headers.Add("App-Id", appId);
                request.Headers.Add("Instance-Id", instanceId);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);

                //var responce = await client.GetAsync(_apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    rResp = await response.Content.ReadAsStringAsync();
                    //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));
                }
                emrPatientData = JsonConvert.DeserializeObject<EMRPatientModel>(rResp);
            }

            return emrPatientData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }).WithTags("EMR").WithMetadata(new SwaggerOperationAttribute("Searches for a patient in the EMR."));


//app.MapPost("/EmrCreatePatient",
//async ([FromHeader]string currentOrganizationId,[FromHeader] string appId, [FromHeader]string instanceId, [FromBody] EMRPatientModel json) =>
//    {
//        string rResp = string.Empty;
//        try
//        {

//            string _apiUrl = ApiConnectionString + "emr/patient";
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
//            request.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
//            request.Headers.Add("Current-Organization-Id", currentOrganizationId);
//            request.Headers.Add("App-Id", appId);
//            request.Headers.Add("Instance-Id", instanceId);
//            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

//            var responce = await client.SendAsync(request);
//            if (responce.IsSuccessStatusCode)
//            {
//                rResp = await responce.Content.ReadAsStringAsync();
//                //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

//            }
//            return rResp;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }).WithTags("EMR").WithMetadata(new SwaggerOperationAttribute("Creates a new patient in the EMR."));

app.MapGet("/EmrIndividualPatientDetails",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async ([FromHeader(Name = "Current-Organization-Id")] string currentOrganizationId, [FromHeader(Name = "App-Id")] string appId, [FromHeader(Name = "Instance-Id")] string instanceId, string patientId) =>
    {
        resource resourceData = null;
        string rResp = string.Empty;
        try
        {
            string uri = apiURL + "RefreshToken";
            var content = new StringContent("");

            //string url = apiURL + $"UserValidate?token1={token}";
            //var resp = await client.GetAsync(url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForAccessAllMethods);
            var resp = await client.GetAsync(uri);
            if (resp.IsSuccessStatusCode)
            {
                string _apiUrl = ApiConnectionString + "emr/patient/" + patientId;
                var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
                request.Headers.Add("Current-Organization-Id", currentOrganizationId);
                request.Headers.Add("App-Id", appId);
                request.Headers.Add("Instance-Id", instanceId);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var responce = await client.SendAsync(request);
                if (responce.IsSuccessStatusCode)
                {
                    rResp = await responce.Content.ReadAsStringAsync();
                    //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

                }
                resourceData = JsonConvert.DeserializeObject<resource>(rResp);
            }

            return resourceData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }).WithTags("EMR").WithMetadata(new SwaggerOperationAttribute("Gets details of a Patient from the EMR."));

//app.MapMethods("/EmrPatientPropertyUpdate", new[] {"PATCH"},
//    async ([FromHeader] string currentOrganizationId, [FromHeader] string appId, [FromHeader]string instanceId, string patientId, [FromBody] EMRPatientModel json) =>
//    {
//        string rResp = string.Empty;
//        try
//        {
//            var stringContent = new StringContent(json.ToString());
//            string _apiUrl = ApiConnectionString + "emr/patient";
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//            var responce = await client.PutAsync(_apiUrl, stringContent);
//            if (responce.IsSuccessStatusCode)
//            {
//                rResp = await responce.Content.ReadAsStringAsync();
//                //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));
//            }
//            return rResp;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }).WithTags("EMR").WithMetadata(new SwaggerOperationAttribute("Updates an properties of an existing patient in the EMR."));

//app.MapPut("/EmrPatientUpdate",
//    async ( string currentOrganizationId, string appId, string instanceId, string patientId,[FromBody] EMRPatientModel json) =>
//    {
//        string rResp = string.Empty;
//        try
//        {
//            var stringContent = new StringContent(json.ToString());
//            string _apiUrl = ApiConnectionString + "emr/patient";
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//            var responce = await client.PutAsync(_apiUrl, stringContent);
//            if (responce.IsSuccessStatusCode)
//            {
//                rResp = await responce.Content.ReadAsStringAsync();
//                //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

//            }
//            resource resourcedata = JsonConvert.DeserializeObject<resource>(rResp);
//            return resourcedata;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }).WithTags("EMR").WithMetadata(new SwaggerOperationAttribute("Updates an existing patient in the EMR."));

app.MapDelete("/EmrDeletePatient",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async ([FromHeader(Name = "Current-Organization-Id")] string currentOrganizationId, [FromHeader(Name = "App-Id")] string appId, [FromHeader(Name = "Instance-Id")] string instanceId, string patientId) =>
    {
        resource resourceData = null;
        string rResp = string.Empty;
        try
        {
            string uri = apiURL + "RefreshToken";
            var content = new StringContent("");

            //string url = apiURL + $"UserValidate?token1={token}";
            //var resp = await client.GetAsync(url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForAccessAllMethods);
            var resp = await client.GetAsync(uri);
            if (resp.IsSuccessStatusCode)
            {
                string json = string.Empty;
                string _apiUrl = ApiConnectionString + "emr/patient/" + patientId;
                var request = new HttpRequestMessage(HttpMethod.Delete, _apiUrl);
                request.Headers.Add("Current-Organization-Id", currentOrganizationId);
                request.Headers.Add("App-Id", appId);
                request.Headers.Add("Instance-Id", instanceId);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    rResp = await response.Content.ReadAsStringAsync();
                    //dynamic data = JsonConvert.DeserializeObject(rResp, typeof(object));

                }
                resourceData = JsonConvert.DeserializeObject<resource>(rResp);
            }

            return resourceData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }).WithTags("EMR").WithMetadata(new SwaggerOperationAttribute("Deletes a patient from the EMR."));


await app.RunAsync();