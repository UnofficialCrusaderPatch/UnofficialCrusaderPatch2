export type RadioOption = {
  description: string;
  type: 'string' | 'color';
  value: string;
}

export type RadioParams = {
  options: ArrayOfTwoOrMore<RadioOption>;
};

export type SliderParams = {
  default: number;
  interval: number;
  maximum: number;
  minimum: number;
  suggested: number;
};

export type SelectionParams = RadioParams | SliderParams;

export type BaseChangeConfig = {
  compatibility? : string[];
  defaultValue: string;
  description: string;
  detailedDescription?: string;
  identifier: string;
}

export type CheckboxChangeConfig = {
  selectionType: 'CHECKBOX'
} & BaseChangeConfig;

export type RadioChangeConfig = {
  selectionType: 'RADIO'
  selectionParameters: RadioParams
} & BaseChangeConfig;

export type SliderChangeConfig = {
  selectionType: 'SLIDER'
  selectionParameters: SliderParams
} & BaseChangeConfig;

export type BackendChangeConfig = CheckboxChangeConfig | RadioChangeConfig | SliderChangeConfig;

export type ArrayOfOneOrMore<T> = [T, ...T[]];
export type ArrayOfTwoOrMore<T> = [T, T, ...T[]];

export type BackendModConfig = {
  changes: ArrayOfOneOrMore<BackendChangeConfig>;
  detailedDescription?: string;
  modDescription: string;
  modIdentifier: string;
  modSelectionRule?: string | null | undefined;
  modType: string;
}

export type ChangeConfig = {
  identifier: string;
  enabled: boolean;
  value?: string | number
}

export type ModConfig = {
  [identifier: string]: ChangeConfig[];
}

export type ModState = {
  [identifier: string]: {
    enabled: boolean;
    value?: string | number;
  }
}