using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OS.Communication;

namespace OS.AutoScanner.Models
{
    public enum DeviceRequestType
    {
        None = -1,
        Connect,
        AllowLongMessages,
        LineFeedsOff,
        SpacesOff,
        ForgetEvents,
        BufferDump,
        DeviceSetDefaults,
        DeviceDescription,
        DeviceReset,
        EchoOff,
        MonitorAll,
        Protocol,
        ProtocolSearch,
        SerialNumber,
        SupplyVoltage,
        MemoryOff,
        CANAddressFilter,
        CANAddressMask,
        CANAutoAddress,
        CANAutoFormatOn,
//        CANSetAddressFilters,
        OBD2_StatusSinceCodesLastCleared,
        OBD2_FreezeFrameCauseFault,
        OBD2_GetEngineRPM,
        OBD2_GetEngineCoolantTemp,
        OBD2_GetEngineLoad,
        OBD2_GetPIDs,
        OBD2_GetVIN,
        OBD2_FuelLevel,
        OBD2_ShortTermFuelTrimBank1,
        OBD2_LongTermFuelTrimBank1,
        OBD2_ShortTermFuelTrimBank2,
        OBD2_LongTermFuelTrimBank2,
        OBD2_FuelSystemStatus,
        OBD2_WarmUpsSinceDTCCleared,
        OBD2_KmSinceDTCCleared,
        OBD2_KmWithMilOn,
        OBD2_GetAmbientTemp,
        OBD2_GetDTCs,
        OBD2_GetPendingDTCs,
        OBD2_ClearDTCs
    }

    public class PIDCategory
    {
        public PIDCategory()
        {
            this.IsSupported = false;
            this.Description = this.Code.ToString();
        }
        public int Code { get; set; }
        public ulong BitMask { get; set; }
        public string Description { get; set; }
        public bool IsSupported { get; set; }
    }

    public class ODB2Device
    {
        private ICommunicationDevice _comDevice = null;
        private readonly bool _scanningPorts = true;

        public ODB2Device(ICommunicationDevice comDevice)
        {
            if (comDevice == null) throw new ArgumentNullException("comDevice");
            this._comDevice = comDevice;
            //      this._ELM327Commands.Add("DP", new ELM327Command { Code = "DP", Name = "Get Protocol" });
        }
        public event DeviceEvent CommunicationEvent
        {
            add
            {
                this._comDevice.CommunicationEvent += value;
            }
            remove
            {
                this._comDevice.CommunicationEvent -= value;
            }
        }
        public bool IsOpen
        {
            get
            {
                if (this._comDevice == null) return false;
                return this._comDevice.IsConnected;
            }
        }
        public bool Open()
        {
            return this._comDevice.Open();
        }
        public string MessageString
        {
            get { return this._comDevice.MessageString; }
        }
        public void Close()
        {
            if (this._comDevice == null) return;
            this._comDevice.Close();

        }

        public bool Send(string data)
        {
            if (this._comDevice == null || !this._comDevice.IsConnected) return false;
            return this._comDevice.Send(data);
        }


        public static readonly Dictionary<DeviceRequestType, ELM327Command> ELM327CommandDictionary = new Dictionary<DeviceRequestType, ELM327Command>
        {
            {DeviceRequestType.EchoOff, new ELM327Command{ IsUserFunction = false, Code = "ATE0", Name = "Отключить эхо", Description = "Отключает ответный эхо" } },
            {DeviceRequestType.BufferDump, new ELM327Command{ IsUserFunction = false, Code = "ATBD", Name = "Buffer Dump", Description = "Perform a buffer dump", RequestType = DeviceRequestType.BufferDump } },
            {DeviceRequestType.AllowLongMessages, new ELM327Command{ IsUserFunction = false, Code = "ATAL", Name = "Allow Long Messages", Description = "Allows messages longer than 7 bytes", RequestType = DeviceRequestType.AllowLongMessages } },
            {DeviceRequestType.LineFeedsOff, new ELM327Command{ IsUserFunction = false, Code = "ATL0", Name = "Line Feeds Off", Description = "Don't return line feeds with <cr>", RequestType = DeviceRequestType.LineFeedsOff } },
            {DeviceRequestType.SpacesOff, new ELM327Command{ IsUserFunction = false, Code = "ATS0", Name = "Spaces Off", Description = "Don't return Spaces with responses", RequestType = DeviceRequestType.SpacesOff } },
            {DeviceRequestType.ForgetEvents, new ELM327Command{ IsUserFunction = false, Code = "ATFE", Name = "Forget Events", Description = "Forget Events like fatal can errors", RequestType = DeviceRequestType.ForgetEvents } },
            {DeviceRequestType.Protocol, new ELM327Command{ Code = "ATDP", Name = "Протокол", Description = "Gets the currently active OBD2 protocol", RequestType = DeviceRequestType.Protocol } },
            {DeviceRequestType.ProtocolSearch, new ELM327Command{ IsUserFunction = false, Code = "ATSP0", Name = "Search for protocol", Description = "Sets device to search for appropriate protocol with vehicle", RequestType = DeviceRequestType.ProtocolSearch } },
            {DeviceRequestType.MemoryOff, new ELM327Command{ IsUserFunction = false, Code = "ATM0", Name = "Memory Off", Description = "Do Not store last used protocol to non-volitile memory", RequestType = DeviceRequestType.MemoryOff } },
            {DeviceRequestType.CANAddressFilter, new ELM327Command{ IsUserFunction = false, Code = "ATCF", Name = "Set CAN address filter", Description = "Set CAN address filter", RequestType = DeviceRequestType.CANAddressFilter } },
            {DeviceRequestType.CANAddressMask, new ELM327Command{ IsUserFunction = false, Code = "ATCM", Name = "Set CAN address mask", Description = "Set CAN address mask", RequestType = DeviceRequestType.CANAddressMask } },
            {DeviceRequestType.CANAutoAddress, new ELM327Command{ IsUserFunction = false, Code = "ATAR", Name = "Set CAN Auto address", Description = "Set CAN Auto address", RequestType = DeviceRequestType.CANAutoAddress } },
            {DeviceRequestType.CANAutoFormatOn, new ELM327Command{ IsUserFunction = false, Code = "ATCAF1", Name = "Auto-format CAN messages", Description = "Auto-format sent and received CAN messages", RequestType = DeviceRequestType.CANAutoFormatOn } },
        //    {DeviceRequestType.CANSetAddressFilters, new ELM327Command{ IsUserFunction = false, Code = "ATCRA", Name = "Set CAN address filters", Description = "Set CAN address filters", RequestType = DeviceRequestType.CANSetAddressFilters } },
            {DeviceRequestType.MonitorAll, new ELM327Command{ Code = "ATMA", Name = "Start Bus Monitor", Description = "Monitors vehicle's data bus", RequestType = DeviceRequestType.MonitorAll } },

            {DeviceRequestType.OBD2_ClearDTCs, new ELM327Command{ Code = "04001", Name = "Стереть DTCs", Description = "Clear OBD2 DTCs", RequestType = DeviceRequestType.OBD2_ClearDTCs } },
            {DeviceRequestType.DeviceReset,  new ELM327Command{ Code = "ATZ", Name = "Reset Device", Description = "Resets the device", RequestType = DeviceRequestType.DeviceReset } },
            {DeviceRequestType.OBD2_GetDTCs, new ELM327Command{ Code = "0300", Name = "Получить коды неисправностей", Description = "Ask vehicle for error codes", RequestType = DeviceRequestType.OBD2_GetDTCs } },
            {DeviceRequestType.OBD2_GetPendingDTCs, new ELM327Command{IsUserFunction = false, Code = "0700", Name = "Get Pending Fault Codes", Description = "Ask vehicle for pending error codes", RequestType = DeviceRequestType.OBD2_GetDTCs } },
            {DeviceRequestType.OBD2_GetVIN, new ELM327Command{ Code = "09023", Name = "Вывести VIN", Description = "Ask vehicle for its Vehice Identification Number (VIN)", RequestType = DeviceRequestType.OBD2_GetVIN,
                                function=(strArray)=>
                                {
                                    string sBuf = "";
                                    foreach (string s in strArray)
                                    {
                                        if (s.Length < 5) continue;
                                        sBuf += s.Substring(2);
                                    }
                                    strArray[0] = sBuf.Substring(6);
                                    sBuf = "";
                                    for(int strIdx = 0; strIdx<33; strIdx+=2)
                                    {
                                        sBuf += (char)byte.Parse(strArray[0].Substring(strIdx,2), System.Globalization.NumberStyles.HexNumber);
                                    }
                                    return sBuf.Trim();
                                } } },
            {DeviceRequestType.DeviceDescription, new ELM327Command{ Code = "AT@1", Name = "Получить описание", Description = "Получает описание от устройства", RequestType = DeviceRequestType.DeviceDescription } },
            {DeviceRequestType.SerialNumber, new ELM327Command{ Code = "AT@2", Name = "Получить серийный номер", Description = "Получает постоянный серийный номер (если запрограммирован)", RequestType = DeviceRequestType.SerialNumber } },
            {DeviceRequestType.SupplyVoltage, new ELM327Command{ Code = "ATRV", Name = "Напряжение в сети", Description = "Получает напряжение подключенного автомобиля", RequestType = DeviceRequestType.SupplyVoltage } },
            {DeviceRequestType.OBD2_GetPIDs, new ELM327Command{ Code = "0100", Name = "Поддерживаемые PID", Description = "Спросите автомобиль о поддерживаемых PIDS", RequestType = DeviceRequestType.OBD2_GetPIDs,
                                function = (strArray)=>
                                {
                                    int byteBufLen = 4;
                                    byte[] byteBuf = new byte[byteBufLen];

                                    string[] socl = new string[byteBufLen];
                                    int i = 0;
                                    var x = 0;
                                    for(i=0;i<byteBufLen;i++)
                                    {
                                        socl[i] = strArray[0].Substring(4+(i*2),2);
                                    }
                                   
                                    for(i = 0;i<byteBufLen;i++)
                                    {
                                        byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
                                    }

                                    x = 0;
                                    uint mask = 0b10000000; // init to binary 10000000
                                    PIDCategory tempPIDCat = null;
                                    StringBuilder sb = new StringBuilder($"Поддерживаемые PIDs:{Environment.NewLine}");
                                    for (i = 0; i < byteBufLen; i++)
                                    {
                                        mask = 0b10000000; // reset to binary 10000000
                                        switch (i)
                                        {
                                            case 0:
                                                for (x = 1; x < 9; x++)
                                                {
                                                    tempPIDCat = ODB2Device.OBD2PIDS[x];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }
                                                    mask >>= 1;
                                                }
                                                break;
                                            case 1:
                                                for (x = 9; x < 17; x++)
                                                {
                                                    tempPIDCat = ODB2Device.OBD2PIDS[x];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }
                                                    mask >>= 1;
                                                }
                                                break;
                                            case 2:
                                                for (x = 17; x < 25; x++)
                                                {
                                                    tempPIDCat = ODB2Device.OBD2PIDS[x];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }
                                                    mask >>= 1;
                                                }
                                                break;
                                            case 3:
                                                for (x = 25; x < 33; x++)
                                                {
                                                    tempPIDCat = ODB2Device.OBD2PIDS[x];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }
                                                    mask >>= 1;
                                                }
                                                break;
                                        }
                                    }
                                    return sb.ToString();
                                } } },
            {DeviceRequestType.OBD2_StatusSinceCodesLastCleared, new ELM327Command{ Code = "0101", Name = "Статус с момента последнего устранения неисправности", Description = "Получает статус с момента последней очистки кодов", RequestType = DeviceRequestType.OBD2_StatusSinceCodesLastCleared,
                                function = (strArray)=>
                                {
                                    int byteBufLen = 4;
                                    byte[] byteBuf = new byte[byteBufLen];

                                    string[] socl = new string[byteBufLen];
                                    int i = 0;
                                    var x = 0;
                                    for(i=0;i<byteBufLen;i++)
                                    {
                                        socl[i] = strArray[0].Substring(4+(i*2),2);
                                    }
                                   
                                    for(i = 0;i<byteBufLen;i++)
                                    {
                                        byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
                                    }

                                    x = 0;
                                    uint mask = 0b10000000; // init to binary 10000000
                                    PIDCategory tempPIDCat = null;
                                    StringBuilder sb = new StringBuilder($"{Environment.NewLine}");
                                    
                                    bool isGasolineEngine = (byteBuf[1] & 0x08) == 0x08; // bit 3 of byte 2 (spark vs compression monitors)
                                    int maxRng = 0;

                                    // for each byte...
                                    for (i = 0; i < byteBufLen; i++)
                                    {
                                        mask = 0b10000000; // reset to binary 10000000
                                        switch (i)
                                        {
                                            case 0:
                                                // bit 7 (mil light)
                                                tempPIDCat = ODB2Device.OBD2MONITORSTATUS[1];
                                                if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                {
                                                    sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                }

                                                int numDTCs = byteBuf[i] & 0x7F; // everything but bit 7 (mil light)
                                                sb.Append($"Количество кодов неисправности: {numDTCs}{Environment.NewLine}");

                                                break;
                                            case 1:

                                                isGasolineEngine = (byteBuf[i] & 0x08) == 0x00; // bit 3 of byte 2 (spark vs compression monitors)

                                                // tests that are incomplete
                                                mask = 0b01000000; // reset to binary 01000000 (bit 6)
                                                for (x = 4; x < 7; x++)
                                                {
                                                    tempPIDCat = ODB2Device.OBD2MONITORSTATUS[x];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }
                                                    mask >>= 1;
                                                }

                                                // tests that are available (complete)
                                                mask = 0b00000100; // reset to binary 00000100 (bit 2)
                                                for (x = 8; x < 0x0B; x++)
                                                {
                                                    tempPIDCat = ODB2Device.OBD2MONITORSTATUS[x];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }
                                                    mask >>= 1;
                                                }
                                                break;
                                            case 2:
                                                // Tests that are available (complete)
                                                if(isGasolineEngine) x = 0x0B;
                                                else x = 0x1B;

                                                maxRng = x + 8;

                                                for (; x < maxRng; x++)
                                                {
                                                    tempPIDCat = ODB2Device.OBD2MONITORSTATUS[x];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }
                                                    mask >>= 1;
                                                }
                                                break;
                                            case 3:
                                                // Tests that are incomplete
                                                if(isGasolineEngine) x = 0x13;
                                                else x = 0x23;
                                                maxRng = x + 8;
                                                for (; x < maxRng; x++)
                                                {
                                                    tempPIDCat = ODB2Device.OBD2MONITORSTATUS[x];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }
                                                    mask >>= 1;
                                                }
                                                break;
                                        }
                                    }
                                    return sb.ToString();
                                } } },
            {DeviceRequestType.OBD2_FreezeFrameCauseFault, new ELM327Command{ Code = "0102", Name = "Стоп-кадр Причина Ошибка", Description = "Получает стоп-кадр, вызывающий ошибку", RequestType = DeviceRequestType.OBD2_FreezeFrameCauseFault,
                                function = (strArray)=>
                                {
                                    return strArray[0];
                                } } },
            {DeviceRequestType.OBD2_FuelSystemStatus, new ELM327Command{ Code = "0103", Name = "Состояние топливной системы", Description = "Получает статус топливной системы автомобиля", RequestType = DeviceRequestType.OBD2_FuelSystemStatus,
                                function = (strArray)=>
                                {
                                    return strArray[0];
                                } } },
            {DeviceRequestType.OBD2_GetEngineCoolantTemp, new ELM327Command{ Code = "0105", Name = "Температура охлаждающей жидкости", Description = "Спросите автомобиль о температуре охлаждающей жидкости", RequestType = DeviceRequestType.OBD2_GetEngineCoolantTemp,
                                function = (strArray)=>
                                {
                                    return int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_GetAmbientTemp, new ELM327Command{ Code = "01461", Name = "Температура на впуске", Description = "Спросите автомобиль о температуре окружающего воздуха", RequestType = DeviceRequestType.OBD2_GetAmbientTemp,
                                function = (strArray)=>
                                {
                                    return int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_ShortTermFuelTrimBank1, new ELM327Command{ Code = "01061", Name = "Краткосрочный FT B1 %", Description = "Кратковременная корректировка подачи топлива для ряда 1 (%)", RequestType = DeviceRequestType.OBD2_ShortTermFuelTrimBank1,
                                function = (strArray)=>
                                {
                                    //(A-128) * 100/128
                                    var t = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_LongTermFuelTrimBank1, new ELM327Command{ Code = "01071", Name = "Долгосрочный FT B1 %", Description = "Долговременная корректировка подачи топлива для ряда 1 (%)", RequestType = DeviceRequestType.OBD2_LongTermFuelTrimBank1,
                                function = (strArray)=>
                                {
                                    //(A-128) * 100/128
                                    var t = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_ShortTermFuelTrimBank2, new ELM327Command{ Code = "01081", Name = "Краткосрочный FT B2 %", Description = "Кратковременная корректировка подачи топлива для банка 2 (%)", RequestType = DeviceRequestType.OBD2_ShortTermFuelTrimBank2,
                                function =  (strArray)=>
                                {
                                    //(A-128) * 100/128
                                    var t = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_LongTermFuelTrimBank2, new ELM327Command{ Code = "01091", Name = "Долгосрочный FT B2 %", Description = "Долгосрочная корректировка подачи топлива для банка 2 (%)", RequestType = DeviceRequestType.OBD2_LongTermFuelTrimBank2,
                                function = (strArray)=>
                                {
                                    //(A-128) * 100/128
                                    var t = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_GetEngineRPM, new ELM327Command{ Code = "010C1", Name = "Обороты двигателя RPM", Description = "Спросите автомобиль об оборотах двигателя", RequestType = DeviceRequestType.OBD2_GetEngineRPM,
                                function = (strArray)=>
                                {
                                    return int.Parse(strArray[0].Substring(4,4), System.Globalization.NumberStyles.HexNumber) / 4;
                                } } },
            {DeviceRequestType.OBD2_GetEngineLoad, new ELM327Command{ Code = "0104", Name = "Мощность двигателя %", Description = "Получает рассчитанный процент нагрузки двигателя транспортного средства", RequestType = DeviceRequestType.OBD2_GetEngineLoad,
                                function = (strArray)=>
                                {
                                    return (int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 100) / 255;
                                } } },
            {DeviceRequestType.OBD2_FuelLevel, new ELM327Command{ Code = "012F1", Name = "Уровень топлива %", Description = "Спросите автомобиль об уровне топлива в процентах", RequestType = DeviceRequestType.OBD2_FuelLevel ,
                                function = (strArray)=>
                                { 
                                    return (int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 100) / 255; 
                                } } },
            {DeviceRequestType.OBD2_WarmUpsSinceDTCCleared, new ELM327Command{ Code = "01302", Name = "Просмотр после сброса DTC", Description = "Спросите автомобиль о количестве прогревов с момента удаления кода неисправности.", RequestType = DeviceRequestType.OBD2_WarmUpsSinceDTCCleared,
                                function = (strArray)=>
                                {
                                    return int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_KmSinceDTCCleared, new ELM327Command{ Code = "01312", Name = "Расстояние с момента удаления DTC", Description = "Расстояние с момента удаления DTC", RequestType = DeviceRequestType.OBD2_KmSinceDTCCleared,
                                function = (strArray)=>
                                {
                                    // (A*256)+B
                                    var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_KmWithMilOn, new ELM327Command{ Code = "01212", Name = "Расстояние с включенным MIL", Description = "Расстояние при включенном MIL", RequestType = DeviceRequestType.OBD2_KmWithMilOn,
                                function = (strArray)=>
                                {
                                    // (A*256)+B
                                    var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);
                                } } }

        };


        // int is the actual PID value, long is the bit value
        static readonly Dictionary<int, PIDCategory> OBD2PIDS = new Dictionary<int, PIDCategory>
        {
            {0x01,  new PIDCategory {Code=0x01, BitMask = (ulong)0x80000000, Description = "Текущие данные" } },
            {0x02,  new PIDCategory {Code=0x02, BitMask = (ulong)0x40000000, Description = "Данные стоп-кадра" } },
            {0x03,  new PIDCategory {Code=0x03, BitMask = (ulong)0x20000000, Description = "Сохраненные коды" } },
            {0x04,  new PIDCategory {Code=0x04, BitMask = (ulong)0x10000000, Description = "Очистить коды" } },
            {0x05,  new PIDCategory {Code=0x05, BitMask = (ulong)0x08000000, Description = "Результаты испытаний и CO02 мониторинг" } },
            {0x06,  new PIDCategory {Code=0x06, BitMask = (ulong)0x04000000, Description = "Результаты испытаний и другой мониторинг" } },
            {0x07,  new PIDCategory {Code=0x07, BitMask = (ulong)0x02000000, Description = "Ожидающие коды" } },
            {0x08,  new PIDCategory {Code=0x08, BitMask = (ulong)0x01000000, Description = "Бортовая система управления" } },
            {0x09,  new PIDCategory {Code=0x09, BitMask = (ulong)0x00800000, Description = "Информация об автомобиле" } },
            {0x0A,  new PIDCategory {Code=0x0A, BitMask = (ulong)0x00400000, Description = "Постоянные коды" } },
            {0x0B,  new PIDCategory {Code=0x0B, BitMask = (ulong)0x00200000, Description = "x0B" } },
            {0x0C,  new PIDCategory {Code=0x0C, BitMask = (ulong)0x00100000, Description = "x0C" } },
            {0x0D,  new PIDCategory {Code=0x0D, BitMask = (ulong)0x00080000, Description = "x0D" } },
            {0x0E,  new PIDCategory {Code=0x0E, BitMask = (ulong)0x00040000, Description = "x0E" } },
            {0x0F,  new PIDCategory {Code=0x0F, BitMask = (ulong)0x00020000, Description = "x0F" } },
            {0x10,  new PIDCategory {Code=0x10, BitMask = (ulong)0x00010000, Description = "x10" } },
            {0x11,  new PIDCategory {Code=0x11, BitMask = (ulong)0x00008000, Description = "x11" } },
            {0x12,  new PIDCategory {Code=0x12, BitMask = (ulong)0x00004000, Description = "x12" } },
            {0x13,  new PIDCategory {Code=0x13, BitMask = (ulong)0x00002000, Description = "x13" } },
            {0x14,  new PIDCategory {Code=0x14, BitMask = (ulong)0x00001000, Description = "x14" } },
            {0x15,  new PIDCategory {Code=0x15, BitMask = (ulong)0x00000800, Description = "x15" } },
            {0x16,  new PIDCategory {Code=0x16, BitMask = (ulong)0x00000400, Description = "x16" } },
            {0x17,  new PIDCategory {Code=0x17, BitMask = (ulong)0x00000200, Description = "x17" } },
            {0x18,  new PIDCategory {Code=0x18, BitMask = (ulong)0x00000100, Description = "x18" } },
            {0x19,  new PIDCategory {Code=0x19, BitMask = (ulong)0x00000080, Description = "x19" } },
            {0x1A,  new PIDCategory {Code=0x1A, BitMask = (ulong)0x00000040, Description = "x1A" } },
            {0x1B,  new PIDCategory {Code=0x1B, BitMask = (ulong)0x00000020, Description = "x1B" } },
            {0x1C,  new PIDCategory {Code=0x1C, BitMask = (ulong)0x00000010, Description = "x1C" } },
            {0x1D,  new PIDCategory {Code=0x1D, BitMask = (ulong)0x00000008, Description = "x1D" } },
            {0x1E,  new PIDCategory {Code=0x1E, BitMask = (ulong)0x00000004, Description = "x1E" } },
            {0x1F,  new PIDCategory {Code=0x1F, BitMask = (ulong)0x00000002, Description = "x1F" } },
            {0x20,  new PIDCategory {Code=0x20, BitMask = (ulong)0x00000001, Description = "x20" } }
        };

        static readonly Dictionary<int, PIDCategory> OBD2MONITORSTATUS = new Dictionary<int, PIDCategory>
        {
            {0x01,  new PIDCategory {Code=0x01, BitMask = (ulong)0x80000000, Description = "Индикатор MIL горит" } },
            {0x02,  new PIDCategory {Code=0x02, BitMask = (ulong)0x7F000000, Description = "Количество подтвержденных кодов неисправности эмиссии" } },
            {0x03,  new PIDCategory {Code=0x03, BitMask = (ulong)0x00800000, Description = "Зарезервировано" } },
            {0x04,  new PIDCategory {Code=0x04, BitMask = (ulong)0x00400000, Description = "Тест компонентов не завершен" } },
            {0x05,  new PIDCategory {Code=0x05, BitMask = (ulong)0x00200000, Description = "Тест топливной системы не завершен" } },
            {0x06,  new PIDCategory {Code=0x06, BitMask = (ulong)0x00100000, Description = "Тест на осечку не завершен" } },
            {0x07,  new PIDCategory {Code=0x07, BitMask = (ulong)0x00080000, Description = "Поддерживаются мониторы сжатия" } },
            {0x08,  new PIDCategory {Code=0x08, BitMask = (ulong)0x00040000, Description = "Проверка компонентов завершена" } },
            {0x09,  new PIDCategory {Code=0x09, BitMask = (ulong)0x00020000, Description = "Тест топливной системы завершен" } },
            {0x0A,  new PIDCategory {Code=0x0A, BitMask = (ulong)0x00010000, Description = "Тест завершен" } },
            // For Spark systems (gasoline)
            {0x0B,  new PIDCategory {Code=0x0B, BitMask = (ulong)0x00008000, Description = "Тест системы EGR завершен" } },
            {0x0C,  new PIDCategory {Code=0x0C, BitMask = (ulong)0x00004000, Description = "Тест нагревателя кислородного датчика завершен" } },
            {0x0D,  new PIDCategory {Code=0x0D, BitMask = (ulong)0x00002000, Description = "Проверка кислородного датчика завершена" } },
            {0x0E,  new PIDCategory {Code=0x0E, BitMask = (ulong)0x00001000, Description = "Тест хладагента кондиционера завершен" } },
            {0x0F,  new PIDCategory {Code=0x0F, BitMask = (ulong)0x00000800, Description = "Тест системы вторичного воздуха завершен" } },
            {0x10,  new PIDCategory {Code=0x10, BitMask = (ulong)0x00000400, Description = "Тест испарительной системы завершен" } },
            {0x11,  new PIDCategory {Code=0x11, BitMask = (ulong)0x00000200, Description = "Испытание нагретого катализатора завершено" } },
            {0x12,  new PIDCategory {Code=0x12, BitMask = (ulong)0x00000100, Description = "Тест катализатора завершен" } },
            {0x13,  new PIDCategory {Code=0x13, BitMask = (ulong)0x00000080, Description = "Тест системы EGR не завершен" } },
            {0x14,  new PIDCategory {Code=0x14, BitMask = (ulong)0x00000040, Description = "Тест нагревателя кислородного датчика не завершен" } },
            {0x15,  new PIDCategory {Code=0x15, BitMask = (ulong)0x00000020, Description = "Тест кислородного датчика не завершен" } },
            {0x16,  new PIDCategory {Code=0x16, BitMask = (ulong)0x00000010, Description = "Тест хладагента кондиционера не завершен" } },
            {0x17,  new PIDCategory {Code=0x17, BitMask = (ulong)0x00000008, Description = "Тест системы вторичного воздуха не завершен" } },
            {0x18,  new PIDCategory {Code=0x18, BitMask = (ulong)0x00000004, Description = "Тест системы испарения не завершен" } },
            {0x19,  new PIDCategory {Code=0x19, BitMask = (ulong)0x00000002, Description = "Тест катализатора с подогревом не завершен" } },
            {0x1A,  new PIDCategory {Code=0x1A, BitMask = (ulong)0x00000001, Description = "Тест катализатора не завершен" } },
            // For compression systems (diesel)
            {0x1B,  new PIDCategory {Code=0x1B, BitMask = (ulong)0x00008000, Description = "Тест системы EGR и/или VVT завершен" } },
            {0x1C,  new PIDCategory {Code=0x1C, BitMask = (ulong)0x00004000, Description = "Мониторинг фильтра" } },
            {0x1D,  new PIDCategory {Code=0x1D, BitMask = (ulong)0x00002000, Description = "Тест датчика выхлопных газов завершен" } },
            {0x1E,  new PIDCategory {Code=0x1E, BitMask = (ulong)0x00001000, Description = "Тест завершен" } },
            {0x1F,  new PIDCategory {Code=0x1F, BitMask = (ulong)0x00000800, Description = "Испытание давлением наддува завершено" } },
            {0x20,  new PIDCategory {Code=0x20, BitMask = (ulong)0x00000400, Description = "Тест завершен" } },
            {0x21,  new PIDCategory {Code=0x21, BitMask = (ulong)0x00000200, Description = "Тест монитора NOx/SCR завершен" } },
            {0x22,  new PIDCategory {Code=0x22, BitMask = (ulong)0x00000100, Description = "Испытание катализатора завершено" } },
            {0x23,  new PIDCategory {Code=0x23, BitMask = (ulong)0x00000080, Description = "Тест системы EGR и/или VVT не завершен" } },
            {0x24,  new PIDCategory {Code=0x24, BitMask = (ulong)0x00000040, Description = "Мониторинг фильтра не завершен" } },
            {0x25,  new PIDCategory {Code=0x25, BitMask = (ulong)0x00000020, Description = "Тест датчика выхлопных газов не завершен" } },
            {0x26,  new PIDCategory {Code=0x26, BitMask = (ulong)0x00000010, Description = "Тест не завершен" } },
            {0x27,  new PIDCategory {Code=0x27, BitMask = (ulong)0x00000008, Description = "Проверка давления наддува не завершена" } },
            {0x28,  new PIDCategory {Code=0x28, BitMask = (ulong)0x00000004, Description = "Тест не завершен" } },
            {0x29,  new PIDCategory {Code=0x29, BitMask = (ulong)0x00000002, Description = "Тест монитора не завершен" } },
            {0x2A,  new PIDCategory {Code=0x2A, BitMask = (ulong)0x00000001, Description = "Тест системы катализатора не завершен" } },


       };

        public static ICollection<ELM327Command> ELM327Commands
        {
            get
            {
             return ELM327CommandDictionary.Values;
            }
        }




    }
}
