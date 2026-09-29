using Projects;


var builder = DistributedApplication.CreateBuilder(args);

// SQL SERVER & DB
var saPassword = builder.AddParameter(
      "sql-sa-password",
      "Local!Dev123",
      secret: true);

var database = builder.AddSqlServer("sqlserver",
                                    password: saPassword,
                                    port: 14330)
                .WithImageTag("2022-latest")
                .WithDataVolume("localinfra_sqlserver-data")
                .WithLifetime(ContainerLifetime.Persistent)                         // survives aspire stop s
                .WithDbGate(dbGate =>
                {
                    dbGate.WithLifetime(ContainerLifetime.Persistent);              // survives aspire stop s
                    dbGate.WithHostPort(64444);
                })
                .AddDatabase("GameStoreDB", "GameStore");                            // connection string name and DB name from app settings.json


// BLOB STORAGE
var blobs = builder.AddAzureStorage("storage")
            .RunAsEmulator(storage =>
            {
                storage.WithBlobPort(10000);
                storage.WithImageTag("3.37.0");
                storage.WithDataVolume("localinfra_azurite-data");
                storage.WithLifetime(ContainerLifetime.Persistent);
            })
            .AddBlobs("Blobs");         // connection string name from app settings.json


// KEY CLOAK
var kcUser = builder.AddParameter("keycloak-admin-user", "admin");
var kcPassword = builder.AddParameter("keycloak-admin-password", "admin", secret: true);

var keycloak = builder.AddKeycloak("keycloak",
                                    port: 8080,
                                    adminUsername: kcUser,
                                    adminPassword: kcPassword)
                .WithImageTag("26.6.1")
                .WithDataVolume("localinfra_keycloak-data")
                .WithRealmImport("../../localinfra")
                .WithLifetime(ContainerLifetime.Persistent);

var keycloakAuthority = ReferenceExpression.Create(
    $"{keycloak.GetEndpoint("http").Property(EndpointProperty.Url)}/realms/gamestore"           // http://localhost:8080/realms/gamestore
);




// API
builder.AddProject<GameStore_Api>("gamestore-api")
        .WithReference(database)
        .WaitFor(database)
        .WithReference(blobs)
        .WaitFor(blobs)
        .WithEnvironment("Authentication__Schemes__KeyCloak__Authority", keycloakAuthority)     // Environment variable containing keycloak authority value - similar to app settings.json
        .WaitFor(keycloak);


builder.Build().Run();
