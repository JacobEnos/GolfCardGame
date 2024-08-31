namespace BlazorServerGolfApp {
    public class CardHand {

        List<List<Card>> Hand;

        public CardHand() {
        }

        public CardHand(List<List<Card>> hand) { 
            Hand = hand;
        }

        public int CountFlipped() {

            int count = 0;
            foreach (List<Card> row in Hand) {
                foreach (Card c in row) {
                    if (c.isShowing) {
                        count++;
                    }
                }
            }
            return count;
        }

        public int GetHandPoints() {
            int handPoints = 0;


            for (int column = 0; column < 3; column++) {
                int columnPoints = 0;

                for (int row = 0; row < 2; row++) {

                    //Ace bonus applies even when column matches
                    if (Hand[row][column].Number == "A") {
                        columnPoints += -5; //Ace bonus
                        continue; //done, move to next card
                    }


                    //if column matches
                    if (Hand[0][column].Number == Hand[1][column].Number) {

                        //equivalent to not changing handPoints
                        //columnPoints = 0;
                        //handPoints += columnPoints;
                        break; //move to next column
                    }
                    //column doesnt match
                    else {
                        int intParse = -10;
                        if(Int32.TryParse(Hand[row][column].Number, out intParse)) {
                            columnPoints += intParse;
                        }
                        else {
                            columnPoints += (Hand[row][column].Number == "K") ? 0 : 10;
                        }
                    }
                }
                handPoints += columnPoints;
            }

            //When done tallying columns, check corners
            if (Hand[0][0].Number == Hand[1][0].Number //col 1 match
                && Hand[0][2].Number == Hand[1][2].Number //col 3 match
                && Hand[0][0].Number == Hand[1][2].Number) { //opposite corner match

                if (Hand[0][0].Number == "A") {
                    handPoints -= 5; //[Corner bonus](25) - [Ace bonus * 4](20)
                }
                else {
                    handPoints -= 25; //columns were tallied as 0, full corner bonus
                }

            }
            return handPoints;
        }

    }
}
