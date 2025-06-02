namespace CraftMyFit.Helpers.Utils
{
    public static class MeasurementUtils
    {
        // Conversioni peso
        public static float KilogramsToPounds(float kg) => kg * 2.20462f;
        public static float PoundsToKilograms(float lbs) => lbs / 2.20462f;

        // Conversioni lunghezza
        public static float CentimetersToInches(float cm) => cm / 2.54f;
        public static float InchesToCentimeters(float inches) => inches * 2.54f;

        // Calcolo BMI
        public static float CalculateBMI(float weightKg, float heightCm)
        {
            if(heightCm <= 0 || weightKg <= 0)
            {
                return 0;
            }

            float heightM = heightCm / 100f;
            return weightKg / (heightM * heightM);
        }

        // Interpretazione BMI
        public static string GetBMICategory(float bmi) => bmi switch
        {
            < 18.5f => "Sottopeso",
            >= 18.5f and < 25f => "Normopeso",
            >= 25f and < 30f => "Sovrappeso",
            >= 30f and < 35f => "Obesità di I grado",
            >= 35f and < 40f => "Obesità di II grado",
            >= 40f => "Obesità di III grado",
            _ => "Non determinato"
        };

        // Calcolo percentuale di grasso corporeo ideale (approssimativo)
        public static (float min, float max) GetIdealBodyFatRange(bool isMale, int age) => isMale
                ? ((float min, float max))(age switch
                {
                    <= 30 => (6f, 14f),
                    <= 40 => (8f, 16f),
                    <= 50 => (10f, 18f),
                    _ => (12f, 20f)
                })
                : ((float min, float max))(age switch
                {
                    <= 30 => (14f, 21f),
                    <= 40 => (16f, 23f),
                    <= 50 => (18f, 25f),
                    _ => (20f, 27f)
                });

        // Calcolo calorie bruciate approssimative durante l'esercizio
        public static float EstimateCaloriesBurned(float weightKg, int durationMinutes, float metValue)
        {
            // Formula: Calorie = MET × peso(kg) × tempo(ore)
            float hours = durationMinutes / 60f;
            return metValue * weightKg * hours;
        }

        // Valori MET comuni per esercizi
        public static float GetMETValue(string exerciseType) => exerciseType.ToLower() switch
        {
            "walking" or "camminata" => 3.8f,
            "running" or "corsa" => 8.0f,
            "cycling" or "ciclismo" => 6.8f,
            "swimming" or "nuoto" => 8.3f,
            "weightlifting" or "pesi" => 6.0f,
            "yoga" => 2.5f,
            "aerobics" or "aerobica" => 7.3f,
            "hiit" => 12.0f,
            "pushup" or "pushups" => 3.8f,
            "situp" or "situps" => 3.8f,
            "squats" => 5.0f,
            "plank" => 3.8f,
            _ => 4.0f // Valore di default
        };

        // Calcolo della percentuale di miglioramento
        public static float CalculateImprovementPercentage(float oldValue, float newValue) => oldValue == 0 ? newValue > 0 ? 100 : 0 : (newValue - oldValue) / oldValue * 100;

        // Formattazione del peso con unità
        public static string FormatWeight(float weight, string unit = "kg") => unit.ToLower() switch
        {
            "kg" => $"{weight:F1} kg",
            "lbs" => $"{weight:F1} lbs",
            _ => $"{weight:F1} {unit}"
        };

        // Formattazione della lunghezza con unità
        public static string FormatLength(float length, string unit = "cm") => unit.ToLower() switch
        {
            "cm" => $"{length:F1} cm",
            "in" => $"{length:F1} in",
            _ => $"{length:F1} {unit}"
        };

        // Calcolo del peso target basato sull'altezza (BMI 22 come target)
        public static float CalculateTargetWeight(float heightCm, float targetBMI = 22f)
        {
            float heightM = heightCm / 100f;
            return targetBMI * (heightM * heightM);
        }

        // Validazione misurazioni
        public static bool IsValidWeight(float weight, string unit = "kg") => unit.ToLower() switch
        {
            "kg" => weight is > 20 and < 300,
            "lbs" => weight is > 44 and < 660,
            _ => weight > 0
        };

        public static bool IsValidHeight(float height, string unit = "cm") => unit.ToLower() switch
        {
            "cm" => height is > 100 and < 250,
            "in" => height is > 39 and < 98,
            _ => height > 0
        };

        // Arrotondamento intelligente per le misurazioni
        public static float RoundMeasurement(float value, int decimals = 1) => (float)Math.Round(value, decimals);
    }
}