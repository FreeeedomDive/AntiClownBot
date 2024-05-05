export const SettingsCategory = {
    DiscordGuild: "DiscordGuild",
    DiscordBot: "DiscordBot",
    Economy: "Economy",
    Shop: "Shop",
    Inventory: "Inventory",
    DailyEvents: "DailyEvents",
    CommonEvents: "CommonEvents"
} as const

export type SettingsCategory = typeof SettingsCategory[keyof typeof SettingsCategory];