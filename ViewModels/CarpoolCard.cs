using CarpooliDotTN.Models;
using System.Security.Claims;
namespace CarpooliDotTN.ViewModels;

public class CarpoolCard
{
    public CarpoolCard(){}
    public CarpoolCard(Carpool carpool){ this.Carpool=carpool;}

    public Carpool Carpool;
    public bool Apply = false;
    public bool SeeApplication = false;
    public bool SeeCarpool = false;
    public bool ByOwner = false;
    public bool Availabilty = false;
    public Guid DemandId ;
    
    public static List<CarpoolCard> GetCardsForOwner(List<Carpool> carpools){
        List<CarpoolCard> carpoolCards = new List<CarpoolCard>();
                    foreach(var carpool in carpools)
                    {
                        CarpoolCard carpoolCard = new CarpoolCard(carpool);
                        carpoolCard.Apply = false;
                        carpoolCard.Availabilty = true;
                        carpoolCard.SeeApplication = false;
                        carpoolCard.SeeCarpool = true;
                        carpoolCard.ByOwner = false;
                        carpoolCards.Add(carpoolCard);
        
                    }
        return carpoolCards;
    }
   
    public static List<CarpoolCard> GetCardsForAll(List<Carpool> carpools, string userId){
        List<CarpoolCard> carpoolCards = new List<CarpoolCard>();
            foreach(var carpool in carpools)
            {
                CarpoolCard carpoolCard = new CarpoolCard(carpool);
                carpoolCard.SeeCarpool = true;
                carpoolCard.Availabilty = true;
                if (carpoolCard.Carpool.OwnerId==userId){
                    carpoolCard.Apply=false;
                    carpoolCard.SeeApplication = false;
                    
                }else {
                    carpoolCard.ByOwner =true;
                    carpoolCard.Apply = true;
                    carpoolCard.SeeApplication=false;
                    foreach (var demand in carpool.Demands)
                    {
                        if (demand.PassengerId == userId)
                        {
                            carpoolCard.Apply = false;
                            carpoolCard.SeeApplication = true;
                            carpoolCard.DemandId = demand.Id;
                            break;
                        }
                    }
                }
                carpoolCards.Add(carpoolCard);

            }

        return carpoolCards;
    }
    
}