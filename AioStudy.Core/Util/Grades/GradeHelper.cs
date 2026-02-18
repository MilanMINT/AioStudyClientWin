using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Util.Grades
{
    public static class GradeHelper
    {
        public static class Math
        {
            public static double CalculateGPA(IEnumerable<Module> modules, bool includeFailedExams)
            {
                double gpa = 0.0;
                int creditsSum = 0;

                foreach (var module in modules)
                {
                    if (module.Grade.HasValue && module.ModuleCredits.HasValue)
                    {
                        if (module.IsKeyCompetence)
                        {
                            continue;
                        }

                        if (!includeFailedExams && module.Grade.Value > 4.0f)
                        {
                            continue;
                        }

                        gpa += module.Grade.Value * module.ModuleCredits.Value * module.Weighting;
                        creditsSum += module.ModuleCredits.Value * module.Weighting;
                    }
                }
                return creditsSum > 0 ? gpa / creditsSum : 0.0;
            }

            public static int CalculateTotalCredits(IEnumerable<Module> modules, bool includeFailedExams)
            {
                int totalCredits = 0;
                foreach (var module in modules)
                {
                    if (module.IsKeyCompetence && module.ModuleCredits.HasValue)
                    {
                        totalCredits += module.ModuleCredits.Value;
                        continue;
                    }

                    if (module.Grade.HasValue && module.Grade.Value > 0 && module.ModuleCredits.HasValue)
                    {
                        if (!includeFailedExams && module.Grade.Value > 4.0f)
                        {
                            continue;
                        }
                        totalCredits += module.ModuleCredits.Value;
                    }
                }
                return totalCredits;
            }
        }
        public static string GetGradeColor(double grade)
        {
            return grade switch
            {
                0.7 => "#22C55E",  // Dunkelgrün
                1.0 => "#4ADE80",  // Grün
                1.3 => "#86EFAC",  // Hellgrün
                1.7 => "#5EEAD4",  // Türkis-Grün
                2.0 => "#2DD4BF",  // Türkis
                2.3 => "#67E8F9",  // Hell-Türkis
                2.7 => "#FACC15",  // Gelb
                3.0 => "#FCD34D",  // Hellgelb
                3.3 => "#FB923C",  // Orange
                3.7 => "#F97316",  // Dunkelorange
                4.0 => "#EF4444",  // Rot
                5.0 => "#DC2626",  // Dunkelrot
                _ => "#6B7280"     // Grau
            };
        }
    }
}