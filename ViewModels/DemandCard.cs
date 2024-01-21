namespace CarpooliDotTN.ViewModels;
using CarpooliDotTN.Models;
public class DemandCard
{
    public Demand Demand;
    public bool Sender=false;
    public bool Carpool=false;
    public bool Creator = false;
    public bool Accept=false;
    public bool Decline=false;
    public bool Cancel=false;
    public bool Header = false;
    public bool State = false;
    public static List<DemandCard> GetCardsForReceiver(List<Demand> demands){
        List<DemandCard> demandCards = new List<DemandCard>();
                        foreach (var demand in demands){
                            DemandCard demandCard = new DemandCard();
                            demandCard.Demand = demand;
                            demandCard.Sender = true;
                            demandCard.Carpool = true;
                            demandCard.Creator = false;
                            demandCard.Accept = demand.status == Demand.Response.pending && demand.Carpool.IsOpen;
                            demandCard.Decline = demand.status == Demand.Response.pending ;
                            demandCard.Cancel = false;
                            demandCard.Header = true;
                            demandCard.State = demand.status != Demand.Response.pending;
                            demandCards.Add(demandCard);
                        }

                        return demandCards;
    }
    public static List<DemandCard> GetCardsForSender(List<Demand> demands){
        List<DemandCard> demandCards = new List<DemandCard>();
                foreach (var demand in demands){
                    DemandCard demandCard = new DemandCard();
                    demandCard.Demand = demand;
                    demandCard.Sender = false;
                    demandCard.Creator = true;
                    demandCard.Carpool = true;
                    demandCard.Accept = false;
                    demandCard.Decline = false;
                    demandCard.Cancel = demand.status != Demand.Response.refused && demand.status!=Demand.Response.cancelled;
                    demandCard.Header = true;
                    demandCard.State = true;
                    demandCards.Add(demandCard);
                }
        return demandCards;
    }
    public static List<DemandCard> GetCardsForReceiverByCarpool(List<Demand> demands){
        List<DemandCard> demandCards = new List<DemandCard>();
                foreach (var demand in demands){
                    DemandCard demandCard = new DemandCard();
                    demandCard.Demand = demand;
                    demandCard.Sender = true;
                    demandCard.Carpool = false;
                    demandCard.Creator = false;
                    demandCard.Accept = demand.status == Demand.Response.pending && demand.Carpool.IsOpen;
                    demandCard.Decline = demand.status == Demand.Response.pending ;
                    demandCard.Cancel = false;
                    demandCard.Header = false;
                    demandCard.State = demand.status != Demand.Response.pending;
                    demandCards.Add(demandCard);
                }
        return demandCards;
    }
    
    
    

}