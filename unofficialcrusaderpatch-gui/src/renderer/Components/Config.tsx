export type ArrayOfOneOrMore<T> = [T, ...T[]];
export type ArrayOfTwoOrMore<T> = [T, T, ...T[]];

// base config shared by all changes in the backend
export type BaseChangeConfig = {
  compatibility? : string[];
  defaultValue: string;
  description: string;
  detailedDescription?: string;
  identifier: string;
}

// config for all changes with only enable/disable
export type CheckboxChangeConfig = {
  selectionType: 'CHECKBOX'
} & BaseChangeConfig;

export type RadioParams = {
  options: ArrayOfTwoOrMore<RadioOption>;
};

export type RadioOption = {
  description: string;
  type: 'string' | 'color';
  value: string;
}

// config for changes with a fixed number of mutually exclusive selectable options
export type RadioChangeConfig = {
  selectionType: 'RADIO'
  selectionParameters: RadioParams
} & BaseChangeConfig;


export type SliderParams = {
  default: number;
  interval: number;
  maximum: number;
  minimum: number;
  suggested: number;
};

// config for changes that allow to select a value from a range
export type SliderChangeConfig = {
  selectionType: 'SLIDER'
  selectionParameters: SliderParams
} & BaseChangeConfig;

// union type of all the different change types
export type BackendChangeConfig = CheckboxChangeConfig | RadioChangeConfig | SliderChangeConfig;

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
  [identifier: string]: ChangeState
}

export type ChangeState = {
  enabled: boolean;
  value?: string | number;
}