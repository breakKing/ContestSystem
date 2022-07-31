using ContestSystem.DbStructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Services
{
    public class LocalizerHelperService
    {
        private readonly List<string> _defaultCultures = new List<string> { "en", "ru" };

        public TLocalizer GetAppropriateLocalizer<TLocalizer>(List<TLocalizer> localizers, string culture)
            where TLocalizer: BaseLocalizer
        {
            if (localizers == null || localizers.Count == 0)
            {
                return null;
            }

            // Берём с запрашиваемой культурой (если есть)
            TLocalizer localizer = localizers.FirstOrDefault(l => l.Culture == culture);

            // С запрашиваемой культурой нет
            if (localizer == null)
            {
                // Найдём одну из дефолтных культур с учётом приоритета (чем раньше культура в массиве, тем она приорететнее)
                for (int i = 0; i < _defaultCultures.Count; i++)
                {
                    localizer = localizers.FirstOrDefault(l => l.Culture == _defaultCultures[i]);
                    if (localizer != null)
                    {
                        break;
                    }
                }

                // Нет локалайзера ни с одной из дефолтных культур - возьмём хоть какой нибудь
                if (localizer == null)
                {
                    localizer = localizers.FirstOrDefault();
                }
            }

            return localizer;
        }
    }
}
