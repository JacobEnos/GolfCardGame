using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorServerGolfApp.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;
using Microsoft.JSInterop;
using Microsoft.Extensions.Hosting;
using Moq;


namespace BlazorServerGolfApp.Hubs.Tests {

    [TestClass()]
    public class GolfHubTests {
        //private TestHost host = new();


        //[TestMethod()]
        //[DataRow()]
        public void GetHandPoints3Test() {

            Card[][] testhand = new Card[][]
            {
                    new Card[]{
                        new("7", "hearts"),
                        new("8", "spades"),
                        new("9", "spades")
                    },
                    new Card[]{
                        new("7", "spades"),
                        new("8", "spades"),
                        new("9", "hearts")
                    }
            };
        }
    }
}