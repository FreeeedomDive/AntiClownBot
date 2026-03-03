export function convertRaceNameToFlag(raceName: string): string {
  if (raceName.startsWith("Бахрейн")) {
    return "https://upload.wikimedia.org/wikipedia/commons/2/2c/Flag_of_Bahrain.svg";
  }
  if (raceName.startsWith("Саудовская Аравия")) {
    return "https://upload.wikimedia.org/wikipedia/commons/0/0d/Flag_of_Saudi_Arabia.svg";
  }
  if (raceName.startsWith("Австралия")) {
    return "https://upload.wikimedia.org/wikipedia/commons/b/b9/Flag_of_Australia.svg";
  }
  if (raceName.startsWith("Япония")) {
    return "https://upload.wikimedia.org/wikipedia/commons/9/9e/Flag_of_Japan.svg";
  }
  if (raceName.startsWith("Китай")) {
    return "https://upload.wikimedia.org/wikipedia/commons/f/fa/Flag_of_the_People%27s_Republic_of_China.svg";
  }
  if (raceName.startsWith("США")) {
    return "https://upload.wikimedia.org/wikipedia/commons/a/a4/Flag_of_the_United_States.svg";
  }
  if (raceName.startsWith("Италия")) {
    return "https://upload.wikimedia.org/wikipedia/commons/0/03/Flag_of_Italy.svg";
  }
  if (raceName.startsWith("Монако")) {
    return "https://upload.wikimedia.org/wikipedia/commons/e/ea/Flag_of_Monaco.svg";
  }
  if (raceName.startsWith("Канада")) {
    return "https://upload.wikimedia.org/wikipedia/commons/d/d9/Flag_of_Canada_%28Pantone%29.svg";
  }
  if (raceName.startsWith("Испания")) {
    return "https://upload.wikimedia.org/wikipedia/commons/9/9a/Flag_of_Spain.svg";
  }
  if (raceName.startsWith("Австрия")) {
    return "https://upload.wikimedia.org/wikipedia/commons/4/41/Flag_of_Austria.svg";
  }
  if (raceName.startsWith("Великобритания")) {
    return "https://upload.wikimedia.org/wikipedia/commons/8/83/Flag_of_the_United_Kingdom_%283-5%29.svg";
  }
  if (raceName.startsWith("Венгрия")) {
    return "https://upload.wikimedia.org/wikipedia/commons/c/c1/Flag_of_Hungary.svg";
  }
  if (raceName.startsWith("Бельгия")) {
    return "https://upload.wikimedia.org/wikipedia/commons/9/92/Flag_of_Belgium_%28civil%29.svg";
  }
  if (raceName.startsWith("Нидерланды")) {
    return "https://upload.wikimedia.org/wikipedia/commons/2/20/Flag_of_the_Netherlands.svg";
  }
  if (raceName.startsWith("Азербайджан")) {
    return "https://upload.wikimedia.org/wikipedia/commons/d/dd/Flag_of_Azerbaijan.svg";
  }
  if (raceName.startsWith("Сингапур")) {
    return "https://upload.wikimedia.org/wikipedia/commons/4/48/Flag_of_Singapore.svg";
  }
  if (raceName.startsWith("Мексика")) {
    return "https://upload.wikimedia.org/wikipedia/commons/f/fc/Flag_of_Mexico.svg";
  }
  if (raceName.startsWith("Бразилия")) {
    return "https://upload.wikimedia.org/wikipedia/commons/0/05/Flag_of_Brazil.svg";
  }
  if (raceName.startsWith("Катар")) {
    return "https://upload.wikimedia.org/wikipedia/commons/6/65/Flag_of_Qatar.svg";
  }
  if (raceName.startsWith("Абу Даби")) {
    return "https://upload.wikimedia.org/wikipedia/commons/c/cb/Flag_of_the_United_Arab_Emirates.svg";
  }

  return "";
}
