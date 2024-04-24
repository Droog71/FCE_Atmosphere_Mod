public class Translator
{
    private readonly string[] detritusNames = {
        "Organic Detritus", "Органический Детрит", "Organické Pozůstatky",
        "Organisches Detritus", "Orgaaninen karike", "Detrito Organico",
        "Détritus Organiques", "有机碎料", "Organik Döküntü"
    };

    private readonly string[] rockNames = { 
        "Rough Hewn Rock", "Roh gehauener Stein", 
        "Roccia Sbozzata", "Roche Taillée Brute", 
        "Raakalouhittu kivi", "Hrubý Kámen",
        "Грубый Камень", "粗岩"
    };

    private readonly string[] snowNames = { 
        "Snow", "Kar", "Schnee",  "Снег", "Śnieg", 
        "Neve", "Neige", "Lumi", "Sníh", "雪"
    };

    public bool IsDetritus(string name)
    {
        for (int i = 0; i < detritusNames.Length; ++i)
        {
            if (name.Equals(detritusNames[i]))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsRock(string name)
    {
        for (int i = 0; i < rockNames.Length; ++i)
        {
            if (name.Equals(rockNames[i]))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsSnow(string name)
    {
        for (int i = 0; i < snowNames.Length; ++i)
        {
            if (name.Equals(snowNames[i]))
            {
                return true;
            }
        }
        return false;
    }
}
