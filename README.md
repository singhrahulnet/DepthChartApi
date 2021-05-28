# Depth Chart Api
A web API built on .NET 5.

### Problem Statement
In INSTRUCTIONS.md

### Setup Details

#### Environment Setup
> Download and install .NET 5
> Download and install VS Code (or Visual Studio) or any editor of your choice

#### Running the application 
> To run the test, open `DepthChartApi.Tests` directory in terminal and run following command

	dotnet test

> To run the application, open `DepthChartApi` directory in terminal and run following command
	
	dotnet run

The application will start serving at `http://localhost:5050/swagger`. Open your browser and hit the URL to see a swagger UI with all the endpoints.


#### Add new supported games
> The supported games are configured in `appsettings.json`. Use the configuration to manage supported games by the api.

```json
"Games": [
    {
      "Name": "NFL",
      "Positions": [
        "QB",
        "WR",
        "RB",
        "TE",
        "K",
        "P",
        "KR",
        "PR"
      ]
    },
    {
      "Name": "MLB",
      "Positions": [
        "SP",
        "RP",
        "C",
        "1B",
        "2B",
        "3B",
        "SS",
        "LF",
        "RF",
        "CF",
        "DH"
      ]
    }
  ]
```

### TODO
- Dockerise
- Automapper for DTOs
- Integration Test

### Considerations

| What        | Thoughts           | Decision  |
| ------------- |:-------------:| -----:|
| Api or console      | Api comes with self documenting tools like swagger. The validations are easier in Api. However, Api is a lot more work | Api |
| Write Algo (DFS/BFS) or use Linq      | The data size is not much. Even if one has to scale this to all international games.      |   Linq|
| Arrange data at the time of persisting or retrieving  | A lot of data movement will be need with option 1     |    Sort data while retrieving |

