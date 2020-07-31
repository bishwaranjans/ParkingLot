using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tricentis
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config()
            {
                CapacitySmall = 2,
                CapacityMedium = 0,
                CapacityLarge = 0
            };

            var requester1 = new Requester
            {
                Id = "Bish1",
                UnluckyNumber = 3,
                Size = 0,// Small
                MaxVehicles = 2

            };
            var requester2 = new Requester
            {
                Id = "Bish2",
                UnluckyNumber = 3,
                Size = 0,// Small
                MaxVehicles = 2

            };
            var requester3 = new Requester
            {
                Id = "Bish3",
                UnluckyNumber = 3,
                Size = 0,// Small
                MaxVehicles = 2

            };
            var requester4 = new Requester
            {
                Id = "Bish4",
                UnluckyNumber = 3,
                Size = 0,// Small
                MaxVehicles = 2

            };

            var manager = new ParkingGarageManager(config);
            var ticket1 = manager.RequestParkingSlot(requester1);
            var ticket2 = manager.RequestParkingSlot(requester2);
            var isLeft = manager.LeaveParkingGarage("djksjs");
            var ticket3 = manager.RequestParkingSlot(requester3);
            var isLeft1 = manager.LeaveParkingGarage(ticket1);
            var ticket4 = manager.RequestParkingSlot(requester4);
            Console.ReadLine();
        }

        public class ParkingGarageManager
        {
            private Dictionary<string, string> _userParkingMapping;
            private List<int> _parkingNumberList;
            public static int CurrentSize;
            private int _parkingNumber;
            private Config _config;
            public ParkingGarageManager(Config config)
            {
                if (config == null)
                {
                    throw new ArgumentException(nameof(config));
                }
                _config = config;
                CurrentSize = 1;
                _userParkingMapping = new Dictionary<string, string>();
                _parkingNumberList = Enumerable.Range(1, _config.Capacity + 1).ToList();
            }

            public string RequestParkingSlot(Requester requester)
            {
                string ticket = null;
                // Check if the size is full or not
                if (CurrentSize < _config.Capacity + 1)
                {
                    // Check if user has one vehicle or not
                    if (_userParkingMapping.ContainsKey(_parkingNumber.ToString()))
                    {
                        return null;
                    }

                    // Check the unlucky number
                    bool foundValidSlot = false;
                    while (!foundValidSlot)
                    {
                        _parkingNumberList.Sort();
                        _parkingNumber = RemoveAndGet(_parkingNumberList, 0);
                        if (_parkingNumber.ToString().Contains(requester.UnluckyNumber.ToString()))
                        {
                            _parkingNumberList.Add(_parkingNumber);
                            continue;
                        }
                        else
                        {
                            foundValidSlot = true;
                        }
                    }

                    ticket = _parkingNumber.ToString();

                    if (!_userParkingMapping.ContainsKey(requester.Id))
                    {
                        _userParkingMapping.Add(requester.Id, ticket);
                    }
                    CurrentSize = CurrentSize + 1;
                    // Allow for store
                }

                return ticket;
            }

            public bool LeaveParkingGarage(string ticket)
            {
                // if a parking found
                if (_userParkingMapping.ContainsValue(ticket))
                {
                    if (int.TryParse(ticket, out _parkingNumber))
                    {
                        CurrentSize = CurrentSize - 1;
                        foreach (var item in _userParkingMapping.Where(kvp => kvp.Value == ticket).ToList())
                        {
                            _userParkingMapping.Remove(item.Key);
                        }

                        // Add back the freed slot
                        _parkingNumberList.Add(_parkingNumber);
                        _parkingNumberList.Sort();
                        return true;
                    }
                    return false;
                }
                return false;
            }

            public static int RemoveAndGet(IList<int> list, int index)
            {
                lock (list)
                {
                    int value = list[index];
                    list.RemoveAt(index);
                    return value;
                }
            }

            /* ---Info---

                public class Requester {
                    public string Id { get; set; }
                    public int UnluckyNumber { get; set; }
                    public int Size { get; set; }
                    public int MaxVehicles { get; set; }
                }

                public class Config {
                    public int Capacity => CapacitySmall + CapacityMedium + CapacityLarge;
                    public int CapacitySmall { get; set; }
                    public int CapacityMedium { get; set; }
                    public int CapacityLarge { get; set; }
                }
            */
        }

        public class Requester
        {
            public string Id { get; set; }
            public int UnluckyNumber { get; set; }
            public int Size { get; set; }
            public int MaxVehicles { get; set; }
        }

        public class Config
        {
            public int Capacity => CapacitySmall + CapacityMedium + CapacityLarge;
            public int CapacitySmall { get; set; }
            public int CapacityMedium { get; set; }
            public int CapacityLarge { get; set; }
        }
    }
}
