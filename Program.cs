using System;
using System.Collections.Generic;
using System.Linq;

namespace MineSweeper // Note: actual namespace depends on the project name.
{
    public class Game
    {
        public static void Main()
        {
            //Label for function to goto in case game is restarted
            restart:
                Console.Write(@"Hi, Welcome to Minesweeper!
Enter anything to begin: ");

                //Take user input to start the game
                string? input = Console.ReadLine();
                //Condition to start game
                if(input is not null && input.Length > 0){
                    Console.Write(@$"Enter the dimensions and amount of bombs you would like to play with (Please enter only digits):
Rows: ");
                    string? yDim = Console.ReadLine();
                    Console.Write("Columns: ");

                    string? xDim = Console.ReadLine();
                    Console.Write("Amount of bombs: ");

                    string? amount = Console.ReadLine();
                    
                    //Ensure the user input contains the appropriate parameters of the correct type
                    if(Int32.TryParse(xDim, out int xDimension) && Int32.TryParse(yDim, out int yDimension) && Int32.TryParse(amount, out int bombs)){
                        
                        //Set up the board based on user inputs
                        char [] board = newBoard(xDimension, yDimension);
                        randomBoard(board, bombs, xDimension, yDimension);
                        computeTouch(board, xDimension, yDimension);
                        hideBoard(board, xDimension, yDimension);

                        //Each turn the game checks whether the game is won, and once it is it proceeds
                        //All loss conditions are checked by the individual member functions, not isGameWon
                        while((isGameWon(board, xDimension, yDimension) == false)){
                            printBoard(board, xDimension, yDimension);
                            //Lets the user either mark a bomb, reveal a location, or restart the game
                            Console.WriteLine("Enter one of the following commands: \"M\"(Mark), \"S\"(Show), \"R\"(Restart)");
                            string? userInput = Console.ReadLine();

                            //If the user indicates they wish to mark a location
                            if(userInput == "M"){
                                Console.Write("Enter the x coordinate of the location you wish to mark: ");
                                string? xInput = Console.ReadLine();

                                Console.Write("Enter the y coordinate of the location you wish to mark: ");
                                string? yInput = Console.ReadLine();

                                //Check the user input for an appropriate input
                                if(Int32.TryParse(xInput, out int xValue) && Int32.TryParse(yInput, out int yValue)){

                                    int markValue = mark(board, xDimension, yDimension, xValue-1, yValue-1);
                                    if(markValue == 2){
                                        Console.WriteLine("Cannot mark a location that has already been revealed.");
                                    }
                                }
                                //If the user input is invalid, notify them
                                else{
                                    Console.WriteLine("Sorry, one or more of your inputs contains an impermissible value (such as a negative or a non-numerical character)");
                                }
                            }

                            //If the user indicates they wish to reveal a location
                            else if(userInput == "S"){
                                Console.Write("Enter the x coordinate of the location you wish to reveal: ");
                                string? xInput = Console.ReadLine();

                                Console.Write("Enter the y coordinate of the location you wish to reveal: ");
                                string? yInput = Console.ReadLine();

                                if(Int32.TryParse(xInput, out int xValue) && Int32.TryParse(yInput, out int yValue)){
                                    int revealValue = reveal(board, xDimension, yDimension, xValue-1, yValue-1);

                                    //Check the various possible returned values of the reveal function for the various exceptions that may occur

                                    if(revealValue == 1){
                                        Console.WriteLine("Cannot reveal a marked location. Please unmark to reveal it (a location can be unmarked by marking an already marked location");
                                    }
                                    else if(revealValue == 2){
                                        Console.WriteLine("This location has already been revealed.");
                                    }
                                    else if(revealValue == 9){
                                        Console.WriteLine("Sorry, you have revealed a location that contains a bomb. \nGame over...");
                                        printBoard(board, xDimension, yDimension);
                                        return;
                                    }
                                    else{

                                    }
                                }
                                //In the case the user input is an invalid input
                                else{
                                    Console.WriteLine("Sorry, one or more of your inputs contains an impermissible value (such as a negative or a non-numerical character)");
                                }
                            }

                            //Jump to restart label if the user wishes to restart the game
                            else if(userInput == "R"){
                                goto restart;
                            }
                            //In the case the user input is an invalid input
                            else{
                                Console.WriteLine("Sorry, this is an invalid input. Please try again.");
                            }
                        }
                        //If the game reaches this point it is because the while loop condition was met (isGameWon function) therefore meaning the user has won the game
                        Console.WriteLine("Congratulations, you have won the game!");
                        //Reveal the entire board to display to user at the end of the game
                        for(int i = 0; i < xDimension; i++){
                            for(int j = 0; j < yDimension; j++){
                                reveal(board, xDimension, yDimension, i, j);
                            }
                        }
                        printBoard(board, xDimension, yDimension);
                    }
                    else{
                        Console.WriteLine("Sorry, one or more of your inputs contains an impermissible value (such as a negative or a non-numerical character)");
                    }
                }
        }
        
        //Function to create a new board from scratch and initialize all locations to '0'
        public static char[] newBoard(int xDim, int yDim){
            char [] board = new char[xDim*yDim];
            for(int i = 0; i < xDim*yDim; i++){
                board[i] = '0';
            }
            return board;
        }

        //Function to add a specified amount of bombs to random locations on the board
        public static void randomBoard(char[] board, int amount, int xDim, int yDim){
            int [] amounts = new int[amount];
            for(int i = 0; i < amount; i++){
                bool repeat = true;
                while(repeat){
                    repeat = false;
                    Random x = new Random();
                    amounts[i] = x.Next(xDim*yDim);
                    //For loop to check if the bomb happens to be placed in the same location again (if it is, repeat the process until it is placed in a unique location)
                    for(int j = 0; j < i; j++){
                        if(amounts[i] == amounts[j]){
                            repeat = true;
                        }
                    }
                }
            }
            //Change the value of these random locations to '9'
            for(int i = 0; i < amount; i++){
                board[amounts[i]] = '9';
            }
        }
        public static void printBoard(char [] board, int xDim, int yDim){
            //Print a row of numbers to label the columns of the board
            for(int i = 0; i < xDim; i++){
                Console.Write(i+1);
                Console.Write(" ");
            }
            Console.Write('\n');
            Console.Write('\n');

            //Create an integer array based on the original character array so they can be manipulated as integers
            int [] newBoard = new int[xDim*yDim];
            for(int i = 0; i < (xDim*yDim); ++i){
                newBoard[i] = board[i] - '0';
            }    

            //For each element, check its value and print the appropriate character to represent it
            for(int i = 0; i < (xDim*yDim); ++i){
                //Print labels at the end of every row to label the rows of the grid
                if(((i%xDim)==0) && (i != 0)){
                    Console.Write("  ");
                    Console.Write(i/xDim);
                    Console.Write('\n');
                }

                //If the character's second hexadecimal value is 3, it is marked, so print M
                if((newBoard[i] & 0x30) == 48)
                {
                    Console.Write("M ");
                }
                //If the character's second hexadecimal value is 2, it is hidden, so print *
                else if((newBoard[i] & 0x20) == 32){
                    Console.Write("* ");;
                }
                //Otherwise, simply print its value with a space after it
                else{
                    if(newBoard[i] == 0x00){
                        Console.Write("0 ");
                    }
                    if(newBoard[i] == 0x01){
                        Console.Write("1 ");
                    }
                    if(newBoard[i] == 0x02){
                        Console.Write("2 ");
                    }
                    if(newBoard[i] == 0x03){
                        Console.Write("3 ");
                    }
                    if(newBoard[i] == 0x04){
                        Console.Write("4 ");
                    }
                    if(newBoard[i] == 0x05){
                        Console.Write("5 ");
                    }
                    if(newBoard[i] == 0x06){
                        Console.Write("6 ");
                    }
                    if(newBoard[i] == 0x07){
                        Console.Write("7 ");
                    }
                    if(newBoard[i] == 0x08){
                        Console.Write("8 ");
                    }
                    if(newBoard[i] == 0x09){
                        Console.Write("9 ");
                    }
                }
                //In the case that this is the last location being printed, label its row (since the original for loop will not)
                if(i == xDim*yDim-1){
                    Console.Write("  ");
                    Console.Write((i+1)/xDim);
                }
            }
            Console.Write('\n');
        }

        //Function that checks each location for how many bombs it is touching
        public static void computeTouch(char [] board, int xDim, int yDim){
            int[] newBoard = new int[xDim*yDim];
            for(int i = 0; i < xDim*yDim; i++){
                newBoard[i] = board[i] - '0';
            }

            for(int i = 0; i < xDim*yDim; i++){
                if(newBoard[i] != 9){
                    int sum = 0;
                    //If the location being checked is in the left most column
                    if(i%xDim == 0){
                        if((i-xDim)>=0){
                            if((newBoard[i-xDim] == 0x09)){
                                sum += 1;
                            }
                        }
                        if((i-xDim)>=0){
                            if((newBoard[i-xDim+1] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(i+xDim<xDim*yDim){
                            if(newBoard[i+xDim] == 0x09){
                                sum += 1;
                            }
                        }
                        if((i+xDim+1)<(xDim*yDim)){
                            if((newBoard[i+xDim+1] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(i+1 < xDim*yDim){
                            if(newBoard[i+1] == 0x09){
                                sum += 1;
                            }
                        }
                    }
                    //If the location being checked is in the right most column
                    if(i%xDim == (xDim - 1)){
                        if(((i-xDim)>=0)){
                            if((newBoard[i-xDim] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(((i-xDim-1)>=0)){
                            if((newBoard[i-xDim-1] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(((i+xDim)<(xDim*yDim))){
                            if((newBoard[i+xDim] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(((i+xDim)<(xDim*yDim))){
                            if((newBoard[i+xDim-1] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(i-1>=0){
                            if(newBoard[i-1] == 0x09){
                                sum += 1;
                            }
                        }
                    }
                    //If the location being checked is not on the furthest outside columns
                    if((i%xDim > 0) && ((i%xDim)<(xDim-1))){
                        if(((i-xDim)>=0)){
                            if((newBoard[i-xDim] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(((i-xDim)>=0)){
                            if((newBoard[i-xDim+1] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(((i-xDim)>=0)){
                            if((newBoard[i-xDim-1] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(((i+xDim)<(xDim*yDim))){
                            if((newBoard[i+xDim] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(((i+xDim)<(xDim*yDim))){
                            if((newBoard[i+xDim+1] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(((i+xDim)<(xDim*yDim))){
                            if((newBoard[i+xDim-1] == 0x09)){
                                sum += 1;
                            }
                        }
                        if(i-1>=0){
                            if(newBoard[i-1] == 0x09){
                                sum += 1;
                            }
                        }
                        if(i+1<=xDim*yDim){
                            if(newBoard[i+1] == 0x09){
                                sum += 1;
                            }
                        }
                    }
                    //Set the location being checked to the sum of all the bombs it is touching
                    newBoard[i] = sum;
                    //Set the corresponding location in the char array to the value assigned in the int array
                    board[i] = (char)(newBoard[i] + '0');
                }
            }
        }

        //Change the second hexadecimal value of each location on the board to 2 (in the integer array), indicating that it is hidden
        public static void hideBoard(char [] board, int xDim, int yDim){
            int[] newBoard = new int[xDim*yDim];
            for(int i = 0; i < (xDim*yDim); i++){
                newBoard[i] = board[i] - '0';
                newBoard[i] = newBoard[i] | 0x20;
                board[i] = (char)(newBoard[i] + '0');    
            }
        }

        //Reveal a specified location on the board (if it is hidden, unhide it by removing the second hexadecimal value of 2 from its integer value)
        public static int reveal(char [] board, int xDim, int yDim, int xLoc, int yLoc){
            int [] newBoard = new int[xDim*yDim];
            for(int i = 0; i < xDim*yDim; i++){
                newBoard[i] = board[i] - '0';
            }
            
            int position = (yLoc*xDim)+xLoc;
            int field = newBoard[position];
            //If the desired location is marked(has a value > 0x30), indicate this by returning 1 which will then be detected by the printBoard function
            if(field >= 0x30){
                return 1;
            }
            //If the desired location is revealed already, return 2 to indicate this
            if(field < 0x09){
                return 2;
            }
            //If the desired location is a bomb, unhide/unmark it if it is hidden or marked, and return 9
            if((field & 0x09) == 0x09){
                newBoard[position] = newBoard[position] % 32;
                newBoard[position] = newBoard[position] % 16;
                for(int j = 0; j < xDim*yDim; j++){
                    board[j] = (char)(newBoard[j] + '0');
                }
                return 9;
            }
            //If the desired location is hidden, unmarked, and not a bomb, reveal it:
            newBoard[position] = newBoard[position] % 32;

            //If the revealed location is touching no bombs, reveal all locations around it as well as they are guaranteed to not be bombs
            if(newBoard[position] == 0){
                //If the revealed location is in the left column, reveal everything that is not on its left
                if((position%xDim) == 0){
                    if((position-xDim) >= 0){
                        newBoard[position-xDim] = newBoard[position-xDim] % 32;
                        newBoard[position-xDim] = newBoard[position-xDim] % 16;
                        newBoard[position-xDim+1] = newBoard[position-xDim+1] % 32;
                        newBoard[position-xDim+1] = newBoard[position-xDim+1] % 16;
                    }
                    newBoard[position+1] = newBoard[position + 1] % 32;
                    newBoard[position+1] = newBoard[position + 1] % 16;
                    if((position+xDim) < (xDim*yDim)){
                        newBoard[position+xDim] = newBoard[position+xDim] % 32;
                        newBoard[position+xDim] = newBoard[position+xDim] % 16;
                        newBoard[position+xDim+1] = newBoard[position+xDim+1] % 32;
                        newBoard[position+xDim+1] = newBoard[position+xDim+1] % 16;
                    }
                }
                //If the revealed location is in the right column, reveal everything that is not on its right
                if((position%xDim) == (xDim-1)){
                    if((position-xDim) >= 0){
                        newBoard[position-xDim] = newBoard[position-xDim] % 32;
                        newBoard[position-xDim] = newBoard[position-xDim] % 16;
                        newBoard[position-xDim-1] = newBoard[position-xDim-1] % 32;
                        newBoard[position-xDim-1] = newBoard[position-xDim-1] % 16;
                    }
                    newBoard[position-1] = newBoard[position - 1] % 32;
                    newBoard[position-1] = newBoard[position - 1] % 16;
                    if((position+xDim) < (xDim*yDim)){
                        newBoard[position+xDim] = newBoard[position+xDim] % 32;
                        newBoard[position+xDim] = newBoard[position+xDim] % 16;
                        newBoard[position+xDim-1] = newBoard[position+xDim-1] % 32;
                        newBoard[position+xDim-1] = newBoard[position+xDim-1] % 16;
                    }
                }
                //If the revealed location is not on the outer columns, reveal all the locations touching it
                if(((position%xDim) < (xDim-1)) && ((position%xDim) > 0)){
                    if((position-xDim) >= 0){
                        newBoard[position-xDim] = newBoard[position-xDim] % 32;
                        newBoard[position-xDim] = newBoard[position-xDim] % 16;
                        newBoard[position-xDim-1] = newBoard[position-xDim-1] % 32;
                        newBoard[position-xDim-1] = newBoard[position-xDim-1] % 16;
                        newBoard[position-xDim+1] = newBoard[position-xDim+1] % 32;
                        newBoard[position-xDim+1] = newBoard[position-xDim+1] % 16;
                    }
                    newBoard[position-1] = newBoard[position - 1] % 32;
                    newBoard[position-1] = newBoard[position - 1] % 16;
                    newBoard[position+1] = newBoard[position + 1] % 32;
                    newBoard[position+1] = newBoard[position + 1] % 16;
                    if((position+xDim) < (xDim*yDim)){
                        newBoard[position+xDim] = newBoard[position+xDim] % 32;
                        newBoard[position+xDim] = newBoard[position+xDim] % 16;
                        newBoard[position+xDim+1] = newBoard[position+xDim+1] % 32;
                        newBoard[position+xDim+1] = newBoard[position+xDim+1] % 16;
                        newBoard[position+xDim-1] = newBoard[position+xDim-1] % 32;
                        newBoard[position+xDim-1] = newBoard[position+xDim-1] % 16;
                    }
                }
            }
            //Convert integer array to char array
            for(int j = 0; j < xDim*yDim; j++){
                board[j] = (char)(newBoard[j] + '0');
            }
            return 0;
        }

        //Function to mark a location the user thinks is a bomb
        public static int mark(char [] board, int xDim, int yDim, int xLoc, int yLoc){
            int [] newBoard = new int[xDim*yDim];
            for(int i = 0; i < xDim*yDim; i++){
                newBoard[i] = board[i] - '0';
            }

            //If the location is already revealed, return 2 to indicate this 
            if(newBoard[(yLoc*xDim)+xLoc] < 0x10){
                return 2;
            }

            //If the location is already marked, unmark it and change the char array to reflect this
            if(newBoard[(yLoc*xDim)+xLoc] >= 0x30){
                newBoard[(yLoc*xDim)+xLoc] = (newBoard[(yLoc*xDim)+xLoc] ^ 0x10);
                for(int i = 0; i < xDim*yDim; i++){
                    board[i] = (char)(newBoard[i] + '0');
                }
                return 0;
            }
            
            //In all other cases, mark the unmarked location
            newBoard[(yLoc*xDim)+xLoc] = newBoard[(yLoc*xDim)+xLoc] | 0x10;
            for(int i = 0; i < xDim*yDim; i++){
                board[i] = (char)(newBoard[i] + '0');
            }
            return 0;
        }

        //Boolean function that checks whether the board is in a won state or not
        public static bool isGameWon(char [] board, int xDim, int yDim){
            int [] newBoard = new int[xDim*yDim];
            for(int i = 0; i < xDim*yDim; i++){
                newBoard[i] = board[i] - '0';
            }

            //If the number of bombs is the same as the number of locations, the game is immediately won
            int sum = 0;
            for(int i = 0; i < xDim*yDim; i++){
                if((newBoard[i] & 0x09) == 0x09){
                    sum += 1;
                }
            }
            if(sum == xDim*yDim){
                return true;
            }

            for(int i = 0; i < xDim*yDim; i++){
                //Check if there are any hidden locations remaining on the board that are not bombs (means the game cannot be won)
                if((newBoard[i] > 0x09) && (newBoard[i] < 0x30) && ((newBoard[i]%32) != 0x09)){
                    return false;
                }
                //Check if there are any revealed bombs (meaning the game is lost)
                if(newBoard[i] == 0x09){
                    return false;
                }
            }
            
            //If every location is either revealed or a bomb (that is not revealed as has already been checked), then the game is won
            for(int i = 0; i < xDim*yDim; i++){
                if((newBoard[i] < 0x10) || ((newBoard[i] & 0x09)== 0x09)){
                    return true;
                }
            }
            
            return false;
        }
    }
}