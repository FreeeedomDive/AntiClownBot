const SUFFIX = "Clown City";

// Более специфичные (длинные) пути должны идти раньше коротких,
// чтобы endsWith не давал ложных совпадений (например /admin/f1Predictions/bingo ⊃ /f1Predictions/bingo).
const PAGE_TITLES: [string, string][] = [
  ["/admin/f1Predictions/results", "Результаты гонок"],
  ["/admin/f1Predictions/championship", "Чемпионат"],
  ["/admin/f1Predictions/bingo", "Бинго"],
  ["/admin/f1Predictions/teams", "Изменение команд"],
  ["/admin/settings", "Настройки"],
  ["/f1Predictions/rulebook", "Регламент"],
  ["/f1Predictions/standings", "Таблица"],
  ["/f1Predictions/races", "Предсказания гонок"],
  ["/f1Predictions/championship", "Чемпионат"],
  ["/f1Predictions/bingo", "Бинго"],
  ["/f1Predictions/statistics", "Статистика"],
  ["/economy", "Экономика"],
  ["/inventory", "Инвентарь"],
  ["/shop", "Магазин"],
  ["/itemsTrade", "Обмен предметами"],
];

export function getPageTitle(pathname: string, userId?: string): string {
  const match = PAGE_TITLES.find(([suffix]) => pathname.endsWith(suffix));
  if (match) {
    return `${match[1]} - ${SUFFIX}`;
  }

  if (
    userId &&
    (pathname === `/user/${userId}` || pathname === `/user/${userId}/`)
  ) {
    return `Профиль - ${SUFFIX}`;
  }

  return SUFFIX;
}
