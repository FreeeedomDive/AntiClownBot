export const F1SafetyCarsPredictionObject = {
  Zero: 'Zero',
  One: 'One',
  Two: 'Two',
  ThreePlus: 'ThreePlus',
} as const;

export type F1SafetyCarPredictionDto =
  (typeof F1SafetyCarsPredictionObject)[keyof typeof F1SafetyCarsPredictionObject];

export const F1SafetyCarsPredictionDto = [
  F1SafetyCarsPredictionObject.Zero,
  F1SafetyCarsPredictionObject.One,
  F1SafetyCarsPredictionObject.Two,
  F1SafetyCarsPredictionObject.ThreePlus,
] as const;
