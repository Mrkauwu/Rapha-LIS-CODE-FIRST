using Microsoft.EntityFrameworkCore;
using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate();

            // Seed Admin User if none exist
            if (!context.Users.Any())
            {
                var adminUser = new UserModel
                {
                    Name = "admin",
                    Age = null,
                    Sex = null,
                    Username = "admin",
                    Role = "Admin",
                    Password = "admin2025",
                    DateCreated = DateTime.Now
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            if (!context.Patients.Any())
            {
                var patients = new List<PatientModel>();

                for (int i = 1; i <= 51; i++)
                {
                    patients.Add(new PatientModel
                    {

                        Name = $"Patient {i}",
                        Age = 20 + (i % 50),  // Ages cycle between 20-69
                        Sex = (i % 2 == 0) ? "Female" : "Male",
                        Physician = $"Dr. Physician {(i % 10) + 1}", // 10 different physicians
                        MedTech = $"MedTech {(i % 15) + 1}",        // 15 different med techs
                        Test = "Hemoglobin",
                        TestResult = (13.5 + (i % 10) * 0.1).ToString("F1") + " g/dL",
                        NormalValue = "13.5-17.5 g/dL",
                        Leukocytes = (4000 + (i % 7000)).ToString(),
                        LeukocytesResult = ((4000 + (i % 7000)) >= 4000 && (4000 + (i % 7000)) <= 11000) ? "Normal" : "Abnormal",
                        LeukocytesNormalValue = "4000-11000 cells/mcL",
                        DateCreated = DateTime.Now.AddDays(-(i % 365)),
                        BarcodeID = $"LAB-2025W21-{i.ToString().PadLeft(5, '0')}"
                    });
                }

                context.Patients.AddRange(patients);
                context.SaveChanges();
            }

            // Seed Tests table if empty
            if (!context.Test.Any())
            {
                var tests = new TestModel[]
                {
        new TestModel { Test = "Hemoglobin", NormalValue = "13.5-17.5 g/dL" },
        new TestModel { Test = "White Blood Cells", NormalValue = "4,000-11,000 cells/mcL" },
        new TestModel { Test = "Platelets", NormalValue = "150,000-450,000/mcL" },

        new TestModel { Test = "Abscess culture", NormalValue = "" },
        new TestModel { Test = "AFB stain", NormalValue = "1200" },
        new TestModel { Test = "GGT", NormalValue = "600" },
        new TestModel { Test = "Gram stain", NormalValue = "1500" },
        new TestModel { Test = "H. pylori Ag (stool)", NormalValue = "800" },
        new TestModel { Test = "PSA-Free", NormalValue = "1200" },
        new TestModel { Test = "PSA-Total", NormalValue = "1200" },
        new TestModel { Test = "PSA profile", NormalValue = "2500" },
        new TestModel { Test = "ALP", NormalValue = "700" },
        new TestModel { Test = "H. pylori Ab (Qualitative)", NormalValue = "1900" },
        new TestModel { Test = "PT (INR)", NormalValue = "900" },
        new TestModel { Test = "Amylase", NormalValue = "450" },
        new TestModel { Test = "H. pylori IgG", NormalValue = "2500" },
        new TestModel { Test = "PTT (APTT)", NormalValue = "900" },
        new TestModel { Test = "ANA (Quantitative)", NormalValue = "3000" },
        new TestModel { Test = "H. pylori IgM", NormalValue = "3000" },
        new TestModel { Test = "Reticulocyte Count", NormalValue = "" },
        new TestModel { Test = "Anti-CCP", NormalValue = "3000" },
        new TestModel { Test = "HCG (Qualitative)", NormalValue = "800" },
        new TestModel { Test = "Rheumatoid Factor", NormalValue = "800" },
        new TestModel { Test = "Anti-TPO", NormalValue = "3000" },
        new TestModel { Test = "HCG (Quantitative)", NormalValue = "900" },
        new TestModel { Test = "RPR (Qualitative)", NormalValue = "800" },
        new TestModel { Test = "Anti-streptolysin O", NormalValue = "1500" },
        new TestModel { Test = "HE4", NormalValue = "1500" },
        new TestModel { Test = "RPR (Quantitative)", NormalValue = "1100" },
        new TestModel { Test = "Anti-Thyroglobulin", NormalValue = "3000" },
        new TestModel { Test = "Hepatitis A IgG", NormalValue = "1000" },
        new TestModel { Test = "Rubella IgG", NormalValue = "1500" },
        new TestModel { Test = "Apolipoprotein A", NormalValue = "2500" },
        new TestModel { Test = "Hepatitis A IgM", NormalValue = "1000" },
        new TestModel { Test = "Rubella IgM", NormalValue = "1500" },
        new TestModel { Test = "Apolipoprotein B", NormalValue = "2500" },
        new TestModel { Test = "Hepatitis B Core (Total)", NormalValue = "900" },
        new TestModel { Test = "Rubeola IgG", NormalValue = "3800" },
        new TestModel { Test = "B-HCG", NormalValue = "1600" },
        new TestModel { Test = "Hepatitis B Core IgM", NormalValue = "1100" },
        new TestModel { Test = "Salmonella IgG/IgM (Typhidot)", NormalValue = "950" },
        new TestModel { Test = "Bicarbonate (CO2)", NormalValue = "500" },
        new TestModel { Test = "Hepatitis B Envelope Ab", NormalValue = "900" },
        new TestModel { Test = "SCC Ag", NormalValue = "1500" },
        new TestModel { Test = "Bilirubin- Direct", NormalValue = "700" },
        new TestModel { Test = "Hepatitis B Envelope Ag", NormalValue = "900" },
        new TestModel { Test = "SHBG", NormalValue = "2300" },
        new TestModel { Test = "Bilirubin-Total", NormalValue = "700" },
        new TestModel { Test = "Hepatitis B Surface Ab", NormalValue = "650" },
        new TestModel { Test = "Sodium", NormalValue = "800" },
        new TestModel { Test = "Bilirubin Package (TB, DB, 18)", NormalValue = "800" },
        new TestModel { Test = "Hepatitis B Surface Ag", NormalValue = "650" },
        new TestModel { Test = "Sputum Culture", NormalValue = "1200" },
        new TestModel { Test = "Blood Culture", NormalValue = "2500" },
        new TestModel { Test = "Hepatitis C Ab", NormalValue = "1300" },
        new TestModel { Test = "Stool Conc. Technique", NormalValue = "800" },
        new TestModel { Test = "Blood Culture with ARD", NormalValue = "2700" },
        new TestModel { Test = "Histology (large)", NormalValue = "2300" },
        new TestModel { Test = "Stool Culture", NormalValue = "1500" },
        new TestModel { Test = "BNP", NormalValue = "2500" },
        new TestModel { Test = "Histology (medium)", NormalValue = "2200" },
        new TestModel { Test = "Surveillance Culture", NormalValue = "1500" },
        new TestModel { Test = "Body Fluid culture", NormalValue = "1800" },
        new TestModel { Test = "Histology (small)", NormalValue = "1800" },
        new TestModel { Test = "Swab Culture", NormalValue = "1500" },
        new TestModel { Test = "CA 125", NormalValue = "2000" },
        new TestModel { Test = "Histology (x-large)", NormalValue = "3800" },
        new TestModel { Test = "Syphilis Ab", NormalValue = "500" },
        new TestModel { Test = "CA 15-3", NormalValue = "2000" },
        new TestModel { Test = "HIV Ag/Ab 182", NormalValue = "1200" },
        new TestModel { Test = "Testosterone", NormalValue = "1300" },
        new TestModel { Test = "CA 19-9", NormalValue = "2000" },
        new TestModel { Test = "Homocysteine", NormalValue = "1900" },
        new TestModel { Test = "Testosterone (Free & Total)", NormalValue = "3500" },
        new TestModel { Test = "Calcium", NormalValue = "500" },
        new TestModel { Test = "hsCRP", NormalValue = "800" },
        new TestModel { Test = "Thyroglobulin", NormalValue = "3600" },
        new TestModel { Test = "CD4", NormalValue = "600" },
        new TestModel { Test = "hs Troponin", NormalValue = "2500" },
        new TestModel { Test = "TIBC", NormalValue = "900" },
        new TestModel { Test = "CEA", NormalValue = "2000" },
        new TestModel { Test = "IgE", NormalValue = "1800" },
        new TestModel { Test = "Total Protein", NormalValue = "650" },
        new TestModel { Test = "Chloride", NormalValue = "500" },
        new TestModel { Test = "India Ink (Cryptococcus)", NormalValue = "800" },
        new TestModel { Test = "Toxoplasma IgG", NormalValue = "1500" },
        new TestModel { Test = "CKMB", NormalValue = "1500" },
        new TestModel { Test = "insulin", NormalValue = "2500" },
        new TestModel { Test = "Toxoplasma IgM", NormalValue = "1500" },
        new TestModel { Test = "Complement 3", NormalValue = "900" },
        new TestModel { Test = "Intact PTH", NormalValue = "2500" },
        new TestModel { Test = "TPA/G", NormalValue = "800" },
        new TestModel { Test = "Cortisol", NormalValue = "1500" },
        new TestModel { Test = "Ionized Calcium", NormalValue = "1000" },
        new TestModel { Test = "TPHA (Quantitative)", NormalValue = "1300" },
        new TestModel { Test = "Cyfra 21-1", NormalValue = "1500" },
        new TestModel { Test = "Iron", NormalValue = "800" },
        new TestModel { Test = "Transferrin", NormalValue = "1500" },
        new TestModel { Test = "Cystatin C", NormalValue = "1800" },
        new TestModel { Test = "KOH Mount", NormalValue = "800" },
        new TestModel { Test = "T-Uptake", NormalValue = "1500" },
        new TestModel { Test = "D-dimer", NormalValue = "5000" },
        new TestModel { Test = "LDH", NormalValue = "600" },
        new TestModel { Test = "Urea", NormalValue = "160" },
        new TestModel { Test = "DHEAS", NormalValue = "2000" },
        new TestModel { Test = "H", NormalValue = "900" },
        new TestModel { Test = "Urine alb/crea ratio", NormalValue = "800" },
        new TestModel { Test = "Lipase", NormalValue = "900" },
        new TestModel { Test = "Urine Amylase", NormalValue = "800" },
        new TestModel { Test = "ESR", NormalValue = "" },
        new TestModel { Test = "ESTRADIOL", NormalValue = "1000" },
        new TestModel { Test = "Lipoprotein a-Lpla", NormalValue = "1800" },
        new TestModel { Test = "Urine Calcium", NormalValue = "700" },
        new TestModel { Test = "FERRITIN", NormalValue = "1200" },
        new TestModel { Test = "Magnesium", NormalValue = "800" },
        new TestModel { Test = "Urine Chloride", NormalValue = "700" },
        new TestModel { Test = "Fluid Cytology", NormalValue = "1900" },
        new TestModel { Test = "Malarial Smear", NormalValue = "800" },
        new TestModel { Test = "Urine Creatinine", NormalValue = "700" },
        new TestModel { Test = "FNAB (per slide)", NormalValue = "1200" },
        new TestModel { Test = "OGCT", NormalValue = "800" },
        new TestModel { Test = "Urine Crea Clearance", NormalValue = "800" },
        new TestModel { Test = "Folic Acid", NormalValue = "1500" },
        new TestModel { Test = "Osmolality (urine)", NormalValue = "800" },
        new TestModel { Test = "Urine Electrolytes", NormalValue = "700" },
        new TestModel { Test = "Free T3", NormalValue = "1300" },
        new TestModel { Test = "Ova and Parasite (O&P)", NormalValue = "800" },
        new TestModel { Test = "Urine Electrolytes (Na,K,Cl)", NormalValue = "700" },
        new TestModel { Test = "Free T4", NormalValue = "1300" },
        new TestModel { Test = "Pap smear", NormalValue = "1000" },
        new TestModel { Test = "Urine Epithelial Cells", NormalValue = "700" },
        new TestModel { Test = "FSH", NormalValue = "800" },
        new TestModel { Test = "Phenytoin (Dilantin)", NormalValue = "1200" },
        new TestModel { Test = "Urine Protein", NormalValue = "700" },
        new TestModel { Test = "FTS/FTA-ABS", NormalValue = "2500" },
        new TestModel { Test = "Plasma renin activity", NormalValue = "1900" },
        new TestModel { Test = "Urine Protein Electrophoresis", NormalValue = "800" },
        new TestModel { Test = "G6PD", NormalValue = "1200" },
        new TestModel { Test = "Plasma renin concentration", NormalValue = "1900" },
        new TestModel { Test = "Urine Protein/creatinine ratio", NormalValue = "800" },
        new TestModel { Test = "GGT", NormalValue = "600" },
        new TestModel { Test = "Platelet Count", NormalValue = "650" },
        new TestModel { Test = "Urine Red Blood Cells", NormalValue = "700" },
        new TestModel { Test = "Gamma GT", NormalValue = "600" },
        new TestModel { Test = "Potassium", NormalValue = "800" },
        new TestModel { Test = "Urine Specific Gravity", NormalValue = "700" },
        new TestModel { Test = "Glycosylated Hemoglobin", NormalValue = "1500" },
        new TestModel { Test = "Procalcitonin", NormalValue = "2300" },
        new TestModel { Test = "Urine Sodium", NormalValue = "700" },
        new TestModel { Test = "Glucose, fasting", NormalValue = "400" },
        new TestModel { Test = "Prolactin", NormalValue = "1200" },
        new TestModel { Test = "Urine Sugar", NormalValue = "800" },
        new TestModel { Test = "Glucose, random", NormalValue = "400" },
        new TestModel { Test = "Protein Electrophoresis", NormalValue = "1500" },
        new TestModel { Test = "Urine Total Protein", NormalValue = "700" },
        new TestModel { Test = "Glycated Hemoglobin (HbA1c)", NormalValue = "1500" },
        new TestModel { Test = "Protein S", NormalValue = "1800" },
        new TestModel { Test = "Urine Urobilinogen", NormalValue = "800" },
        new TestModel { Test = "HBA1C", NormalValue = "1500" },
        new TestModel { Test = "Protein C", NormalValue = "1800" },
        new TestModel { Test = "Urine WBC", NormalValue = "700" },
        new TestModel { Test = "HBeAg", NormalValue = "900" },
        new TestModel { Test = "Prothrombin time", NormalValue = "700" },
        new TestModel { Test = "VDRL", NormalValue = "900" },
        new TestModel { Test = "HBeAb", NormalValue = "900" },
        new TestModel { Test = "PSA", NormalValue = "800" },
        new TestModel { Test = "Vitamin B12", NormalValue = "1500" },
        new TestModel { Test = "HBsAg", NormalValue = "650" },
        new TestModel { Test = "PT (Prothrombin Time)", NormalValue = "700" },
        new TestModel { Test = "Vitamin D 25-OH Total", NormalValue = "1500" },
        new TestModel { Test = "HCV RNA Quantitative", NormalValue = "2800" },
        new TestModel { Test = "PTT (Partial Thromboplastin Time)", NormalValue = "700" },
        new TestModel { Test = "Widal Test", NormalValue = "700" },
        new TestModel { Test = "Hepatitis B Surface Antigen", NormalValue = "650" },
        new TestModel { Test = "TSH", NormalValue = "800" },
        new TestModel { Test = "Zika Virus RNA", NormalValue = "3500" }
                };

                context.Test.AddRange(tests);
                context.SaveChanges();
            }


            if (!context.Leukocytes.Any())
            {
                var leukocytes = new LeukocytesModel[]
                {
                    new LeukocytesModel { Leukocytes = "Segments", LeukocytesNormalValue = "0.55-0.65" },
                    new LeukocytesModel { Leukocytes = "Band", LeukocytesNormalValue = "0.02-0.04" },
                    new LeukocytesModel { Leukocytes = "Basophil", LeukocytesNormalValue = "0.00,0.01" },
                    new LeukocytesModel { Leukocytes = "Lymphocytes", LeukocytesNormalValue = "0.25-0.35" },
                    new LeukocytesModel { Leukocytes = "Monocytes", LeukocytesNormalValue = "0.02-0.06" },
                    new LeukocytesModel { Leukocytes = "Eosinophil", LeukocytesNormalValue = "0.02-0.04" },
                    new LeukocytesModel { Leukocytes = "Juvenile", LeukocytesNormalValue = "0.00-0.02" }
                };

                context.Leukocytes.AddRange(leukocytes);
                context.SaveChanges();
            }
        }
    }
}
