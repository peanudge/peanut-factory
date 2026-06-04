export const queryKeys = {
  cameras:           ["cameras"]                         as const,
  acquisitionStatus: ["acquisitionStatus"]               as const,
  latestFrame:       ["latestFrame"]                     as const,
  boards:            ["boards"]                          as const,
  boardStatus:       (index: number) => ["boardStatus", index] as const,
  presets:           ["presets"]                         as const,
  histogram:         ["histogram"]                       as const,
  imageSaveSettings: ["imageSaveSettings"]               as const,
  exposure:          ["exposure"]                        as const,
  images:            (params?: object) => params ? ["images", params] as const : ["images"] as const,
};
