# Carpooli.Tn

Welcome to Carpooli.tn - Allowing users to share  car journeys so that more than one person travels in a car, and prevents the need for others to have to drive to a location themselves
## Getting Started
Database creation and connection are handled at startup so don't worry about it.

To get started, just follow these steps:

1. Clone the repository.
2. Make sure that SQL Server is installed and running.
3. Install `dotnet ef` if you don't have it: `dotnet tool install --global dotnet-ef` command.
4. Run `dotnet ef database update` to update your database to the latest migrations.

## Usage
### Users and Authentication
- Users are able to create accounts and authenticate.
- Only authenticated Users can join existing carpools and/or create new one.
- Unauthenticated user can only see available carpools, no more, no less. They will be asked to LogIn/SignUp when tries to do unauthorised actions.
### Carpools
- There is a List of Carpools public for everyone.
- A carpool is either open or close for demand.
- Carpool creator is able to close demands for a carpool, and end offer, and re-open it after being closed too.
- When creating Carpool, the author picks starting place, destination, time of departure, cost per each, and number of available places, (optional: time between trips should be calculated using Google Maps API)
- When attempting to join a carpool, a user must choose the pick up place (where to get picked up), his profile infos will be transferred with the demand, and additional text for additional informations is optional.
- You can subscribe only one time for each carpool.
- when the author accepts a passenger, the number of places left is decremented. 
- optional: messaging system between Users and Drivers.
- every profile is public, driver can see those who demand joining, and can respond to any one by refusal (with a reason behind refusal, can choose one from a list or write exactly why, or even choose not to send message) and accepting, when places are full and confirmed, the carpool offer is closed and the rest of the demands will be refused with reason "no places are left". 
- optional: Rating system, for both.