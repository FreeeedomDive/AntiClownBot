const flags: [string[], string][] = [
  [
    ["Бахрейн", "Bahrain"],
    "https://upload.wikimedia.org/wikipedia/commons/2/2c/Flag_of_Bahrain.svg",
  ],
  [
    ["Малайзия", "Malaysia"],
    "https://upload.wikimedia.org/wikipedia/commons/6/66/Flag_of_Malaysia.svg",
  ],
  [
    ["Саудовская Аравия", "Saudi Arabia", "Saudi Arabian", "Jeddah"],
    "https://upload.wikimedia.org/wikipedia/commons/0/0d/Flag_of_Saudi_Arabia.svg",
  ],
  [
    ["Австралия", "Australia", "Australian", "Melbourne"],
    "https://upload.wikimedia.org/wikipedia/commons/b/b9/Flag_of_Australia.svg",
  ],
  [
    ["Япония", "Japan", "Japanese", "Suzuka"],
    "https://upload.wikimedia.org/wikipedia/commons/9/9e/Flag_of_Japan.svg",
  ],
  [
    ["Китай", "China", "Chinese", "Shanghai"],
    "https://upload.wikimedia.org/wikipedia/commons/f/fa/Flag_of_the_People%27s_Republic_of_China.svg",
  ],
  [
    ["США", "United States", "Miami", "Las Vegas", "Austin"],
    "https://upload.wikimedia.org/wikipedia/commons/a/a4/Flag_of_the_United_States.svg",
  ],
  [
    ["Италия", "Italy", "Italian", "Emilia Romagna", "Imola", "Monza"],
    "https://upload.wikimedia.org/wikipedia/commons/0/03/Flag_of_Italy.svg",
  ],
  [
    ["Монако", "Monaco"],
    "https://upload.wikimedia.org/wikipedia/commons/e/ea/Flag_of_Monaco.svg",
  ],
  [
    ["Канада", "Canada", "Canadian", "Montreal"],
    "https://upload.wikimedia.org/wikipedia/commons/d/d9/Flag_of_Canada_%28Pantone%29.svg",
  ],
  [
    ["Испания", "Spain", "Spanish", "Barcelona", "Madrid", "Madring"],
    "https://upload.wikimedia.org/wikipedia/commons/9/9a/Flag_of_Spain.svg",
  ],
  [
    ["Австрия", "Austria", "Austrian", "Spielberg"],
    "https://upload.wikimedia.org/wikipedia/commons/4/41/Flag_of_Austria.svg",
  ],
  [
    ["Великобритания", "Great Britain", "British", "Silverstone"],
    "https://upload.wikimedia.org/wikipedia/commons/8/83/Flag_of_the_United_Kingdom_%283-5%29.svg",
  ],
  [
    ["Венгрия", "Hungary", "Hungarian", "Budapest"],
    "https://upload.wikimedia.org/wikipedia/commons/c/c1/Flag_of_Hungary.svg",
  ],
  [
    ["Бельгия", "Belgium", "Belgian", "Spa"],
    "https://upload.wikimedia.org/wikipedia/commons/9/92/Flag_of_Belgium_%28civil%29.svg",
  ],
  [
    ["Нидерланды", "Netherlands", "Dutch", "Zandvoort"],
    "https://upload.wikimedia.org/wikipedia/commons/2/20/Flag_of_the_Netherlands.svg",
  ],
  [
    ["Азербайджан", "Azerbaijan", "Baku"],
    "https://upload.wikimedia.org/wikipedia/commons/d/dd/Flag_of_Azerbaijan.svg",
  ],
  [
    ["Сингапур", "Singapore"],
    "https://upload.wikimedia.org/wikipedia/commons/4/48/Flag_of_Singapore.svg",
  ],
  [
    ["Мексика", "Mexico", "Mexican"],
    "https://upload.wikimedia.org/wikipedia/commons/f/fc/Flag_of_Mexico.svg",
  ],
  [
    ["Бразилия", "Brazil", "Brazilian", "São Paulo", "Sao Paulo"],
    "https://upload.wikimedia.org/wikipedia/commons/0/05/Flag_of_Brazil.svg",
  ],
  [
    ["Катар", "Qatar", "Lusail"],
    "https://upload.wikimedia.org/wikipedia/commons/6/65/Flag_of_Qatar.svg",
  ],
  [
    ["Абу Даби", "Abu Dhabi", "Yas Marina"],
    "https://upload.wikimedia.org/wikipedia/commons/c/cb/Flag_of_the_United_Arab_Emirates.svg",
  ],
];

export function convertRaceNameToFlag(raceName: string): string {
  return (
    flags.find(([prefixes]) =>
      prefixes.some((prefix) => raceName.startsWith(prefix)),
    )?.[1] ?? ""
  );
}
