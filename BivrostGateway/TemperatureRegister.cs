using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BivrostGateway
{
    class TemperatureRegister
    {
        private int m_hardwareId;
        private int m_sensorId;
        private DateTimeOffset m_registerTime;
        private double m_temperature;

        public TemperatureRegister(int m_hardwareId, int m_sensorId, DateTimeOffset m_registerTime, double m_temperature)
        {
            this.m_hardwareId = m_hardwareId;
            this.m_sensorId = m_sensorId;
            this.m_registerTime = m_registerTime;
            this.m_temperature = m_temperature;
        }

        public int HardwareId
        {
            get
            {
                return m_hardwareId;
            }

            set
            {
                m_hardwareId = value;
            }
        }

        public int SensorId
        {
            get
            {
                return m_sensorId;
            }

            set
            {
                m_sensorId = value;
            }
        }

        public DateTimeOffset RegisterTime
        {
            get
            {
                return m_registerTime;
            }

            set
            {
                m_registerTime = value;
            }
        }

        public double Temperature
        {
            get
            {
                return m_temperature;
            }

            set
            {
                m_temperature = value;
            }
        }
    }
}
