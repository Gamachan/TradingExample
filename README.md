# Java/C# Feed Handlers Programming Test

## Workflow
1. Fork this repository
2. Create a *src* folder to contain your code
3. In the *src* directory, please create a Java or C# application that fulfills the [Application Specs](#application-specs)
4. Commit and push your code to your new repository, submit a pull request
5. We will review your submission and get back to you

## Application Specs
* Download the [LOBSTER AMZN 5 level sample data file](https://lobster.wiwi.hu-berlin.de/info/sample/LOBSTER_SampleFile_AMZN_2012-06-21_5.zip)
* Parse the "orderbook" file into timestamped OrderBook objects (hint: timestamps can be retrieved from the "message" file). This represents the most recent state of the market's visible order book (the "market data"); total order quantity at each price levels
 * Design the OrderBook class as you see fit
* Upon processing each OrderBook, perform the following actions:
 * Submit a new order (NewOrderSingle), if one is not already outstanding (a NewOrderSingle that was previously submitted and not yet cancelled), with a quantity of 100 to join any price level in the most recent OrderBook where the visible quantity is greater than 500
 * Submit a cancel (with an OrderCancel) for any outstanding orders if they are at a price level which no longer has a visible quantity of at least 200
* To send/submit an order, we will just "fake" it: create the object, serialize it (however you like) and send it out a TCP socket

### Class Pseudocode
```
  class OrderMessage
   long sendingTimestampUTC
   String uniqueOrderID
  
  class NewOrderSingle : OrderMessage
   long price
   long quantity
   SideEnum side
  
  enum SideEnum
   Buy, Sell
  
  class OrderCancel : OrderMessage
   String orderIDofOrderToCancel
```

## Assessment Criteria:

1. Each row within message file corresponds to row within orderbook file.
2. LOBSTER_SampleFile_AMZN_2012-06-21_5 was downloaded and saved into Data folder.
3. I am using CSVHelper open source library to handle CSV parsing. CSVHelper: http://joshclose.github.io/CsvHelper/
4. I am using external dll for logging.
5. Application contains App.config file with path to Log file and path to input data folder. 
6. Application can handle more than one folder within input.
7. Input files should have the same meaningful and relevant input file names. 

## Testing:
1. Set irrelevant folder path => handle exception properly
2. Files in folder  open in another process => handle exception properly
3. One file is missing => handle exception properly
4. Number of lines in message file doesn't correspond number of files in orderbook file => handle exception properly
5. Order book size never exceed 500 => no orders were submitted
6. Order book size never drops lower than 200 => no order cancellation was submitted
7. Tcp handler test:
 - no tcp connection was established => based on specification (retry, skip,...etc) 
 - submission or cancellation failed => based on specification (retry, skip,...etc)
 - in case of TCP failed => hander order book collection properly
 - check no cancellation was send for the order which failed to be submitted
 






