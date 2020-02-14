using System;
using COMMON;

namespace Machine
{
    public class USB_Spectrometer
    {
        OmniDriver.NETWrapper       wrapper             = null;
        //SPAM.NETAdvancedPeakFinding advancedPeakFinding ;//= null;
        //SPAM.NETSpectralMath        spectralMath        ;//= null;
        //SPAM.CCoAdvancedPeakFinding advancedPeakFinding = null;
        SPAM.CCoSpectralMath        spectralMath        ;

        int buildNumber = 0;
	    int  minimumAllowedIntegrationTime = 400000; // units: microseconds
	    int  numberOfPixels = 0;
	    int  numberOfSpectrometersAttached = 0; // actually attached and talking to us
	    int  spectrometerIndex = 0; // 0-n, identifies which spectrometer we are interacting with
        bool Opened = false;

        string apiVersion;
        string firmwareVersion;
        string spectrometerName; 
        string serialNumber;
        //WRAPPER_T wrapperHandle;
        //DOUBLEARRAY_T wavelengthArrayHandle;
        //DOUBLEARRAY_T spectrumArrayHandle;
        double[] wavelengthValues;
        double[] spectrumValues;
        public double dCenterWaveLength;
        public double dMaxSpectrumVal;

        public USB_Spectrometer()
        {
            Init();
            if(!Open())
            {
                Log.ShowMessage("Spectrometer", "스펙트로미터의 연결을 확인 하세요.");
            }
        }

        public void Init()
        {
            wrapper = new OmniDriver.NETWrapper();
            //advancedPeakFinding = new SPAM.NETAdvancedPeakFinding();
            //spectralMath = new SPAM.NETSpectralMath();
            //advancedPeakFinding = new SPAM.CCoAdvancedPeakFinding();
            spectralMath = new SPAM.CCoSpectralMath();

            buildNumber = 0;
            minimumAllowedIntegrationTime = 400000; // units: microseconds
            numberOfPixels = 0;
            numberOfSpectrometersAttached = 0; // actually attached and talking to us
            spectrometerIndex = 0; // 0-n, identifies which spectrometer we are interacting with
            Opened = false;
        }
        
        public bool Open()
        {
            //
            //if (Opened) return true;
            if (numberOfSpectrometersAttached > 0)
                wrapper.closeAllSpectrometers();

            //wrapperHandle = wrapper.CreateWrapper();    //Wrapper_Create_stdcall();
            buildNumber = wrapper.getBuildNumber();     //Wrapper_getBuildNumber_stdcall(wrapperHandle);
            apiVersion = wrapper.getApiVersion();       //JString_Create_stdcall();
            //Wrapper_getApiVersion_stdcall(wrapperHandle, apiVersion);
            //JString_Destroy_stdcall(apiVersion);

            // The following call will populate an internal array of spectrometer objects
            numberOfSpectrometersAttached = wrapper.openAllSpectrometers();//Wrapper_openAllSpectrometers_stdcall(wrapperHandle);
            if (numberOfSpectrometersAttached < 1) return false;
            //if (numberOfSpectrometersAttached == 0) return false; // there are no attached spectrometers

            // We will arbitrarily use the first attached spectrometer
            spectrometerIndex = 0;

            // Display some information about this spectrometer
            //firmwareVersion = JString_Create_stdcall();
            firmwareVersion  = wrapper.getFirmwareVersion(spectrometerIndex);//JString_Create_stdcall();
            serialNumber     = wrapper.getSerialNumber(spectrometerIndex);//JString_Create_stdcall();
            spectrometerName = wrapper.getName(spectrometerIndex);//JString_Create_stdcall();
            //Wrapper_getSerialNumber_stdcall(wrapperHandle, spectrometerIndex, serialNumber);
            //Wrapper_getName_stdcall(wrapperHandle, spectrometerIndex, spectrometerName);
            //Wrapper_getFirmwareVersion_stdcall(wrapperHandle, spectrometerIndex, firmwareVersion);
            //JString_Destroy_stdcall(firmwareVersion);
            //JString_Destroy_stdcall(serialNumber);
            //JString_Destroy_stdcall(spectrometerName);

            // Set some acquisition parameters
            minimumAllowedIntegrationTime = wrapper.getMinimumIntegrationTime(spectrometerIndex);//Wrapper_getMinimumIntegrationTime_stdcall(wrapperHandle, spectrometerIndex);
            //minimumAllowedIntegrationTime = LotInfo.SeperInterTime; //5000  = 5ms
            wrapper.setIntegrationTime         (spectrometerIndex, minimumAllowedIntegrationTime);//Wrapper_setIntegrationTime_stdcall(wrapperHandle, spectrometerIndex, minimumAllowedIntegrationTime);
            wrapper.setBoxcarWidth             (spectrometerIndex, 0                            );//Wrapper_setBoxcarWidth_stdcall(wrapperHandle, spectrometerIndex, 0);
            wrapper.setScansToAverage          (spectrometerIndex, 1                            );//Wrapper_setScansToAverage_stdcall(wrapperHandle, spectrometerIndex, 1);
            wrapper.setCorrectForElectricalDark(spectrometerIndex, 0                            );//Wrapper_setCorrectForElectricalDark_stdcall(wrapperHandle, spectrometerIndex, 0);

            Opened = true;

            //이상하게 오픈하고 첫타가 값이 튐.
            Aquire();
            CalPeakVal();

            return true;
        }

        
        public void Aquire()
        {
            // Aquire a spectrum
            //spectrumArrayHandle = DoubleArray_Create_stdcall();

            //wrapper.getSpectrum(spectrometerIndex);

            spectrumValues = wrapper.getSpectrum(spectrometerIndex);//Wrapper_getSpectrum_stdcall(wrapperHandle, spectrometerIndex, spectrumArrayHandle);
            //spectrumValues = DoubleArray_getDoubleValues_stdcall(spectrumArrayHandle);
            numberOfPixels = wrapper.getNumberOfPixels(spectrometerIndex);//DoubleArray_getLength_stdcall(spectrumArrayHandle);
        
            //wavelengthArrayHandle = (DOUBLEARRAY_T)DoubleArray_Create_stdcall();
            wavelengthValues = wrapper.getWavelengths(spectrometerIndex);//Wrapper_getWavelengths_stdcall(wrapperHandle, spectrometerIndex, wavelengthArrayHandle);
            //wavelengthValues = DoubleArray_getDoubleValues_stdcall(wavelengthArrayHandle);
        }
        
        public void CalPeakVal()
        {
            // Spectrum Peak
            int indexOfPeak;
            int minimumIndicesBetweenPeaks;
            int numberOfSpectrometers;
            int peakIndex;
            int startingIndex;
            double baseline;
            string str;

            double[] spectrum;

            if (spectrometerIndex == -1)return  ; // no available spectrometer
            numberOfPixels = wrapper.getNumberOfPixels(spectrometerIndex);

            // Set some acquisition parameters and then acquire a spectrum
            wrapper.setIntegrationTime(spectrometerIndex, 500000); // .5 seconds
            wrapper.setBoxcarWidth(spectrometerIndex, 10);
            wrapper.setCorrectForElectricalDark(spectrometerIndex, 1);
            spectrum = (double[])wrapper.getSpectrum(spectrometerIndex);

            SPAM.CCoAdvancedPeakFinding advancedPeakFinding;
            advancedPeakFinding = spectralMath.createAdvancedPeakFindingObject();

            startingIndex = 0;
            minimumIndicesBetweenPeaks = 1500;
            baseline = 700;
        
            do
            {
                indexOfPeak = advancedPeakFinding.getNextPeakIndex(spectrum, startingIndex, minimumIndicesBetweenPeaks, baseline);
                if (indexOfPeak == 0)
                    break;
                startingIndex = indexOfPeak;
            } while (indexOfPeak > 0);
        
            dMaxSpectrumVal   = spectrumValues  [473];//spectrumValues  [startingIndex];
            dCenterWaveLength = wavelengthValues[473];//wavelengthValues[startingIndex];

            //옵션으로 빼야 될것.
            //const double dTemp = 0.1 ;
            //const int    iTemp = 1 ;
            //return (spectrumValues  [startingIndex] / dTemp) - iTemp ;


            //str.printf("%.1f", spectrumValues[startingIndex]); MaxSpectrumVal = str;
            //str.printf("%.1f", wavelengthValues[startingIndex]); CenterWaveLength = str;
        }
        
        public double GetCalHemoglobin(double _dBlank , double _dAngle)
        {
            string str;
            double dTemp;
            if (dCenterWaveLength < 500 || dCenterWaveLength > 600) return 0.0;

            dTemp = (_dBlank - dMaxSpectrumVal) / _dAngle; // 24.5;
            dTemp = Math.Round(dTemp, 2);
            //int iTemp = (int)(dTemp * 100); //   Math.Round(dTemp , 2) ;

            return dTemp;
        } 
        /*
         Seq Step 1 : Aquire();                 //분광기 검사시작
         Seq Step 2 : CalPeakVal();             //특정 파장대 값 취득  사용파장 (550nm)
         Seq Step 3: str = GetCalHemoglobin();  //헤모글로빈 값으로 변환(공식
         공식 : 헤모글로빈 값 = (Blank – 측정값) / 상수

         */
    }
}
