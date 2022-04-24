export type ChangeConfig = {
  identifier: string;
  value?: string | number
}

export type ModConfig = {
  [identifier: string]: ChangeConfig[];
}

const initialState: ModConfig = {};

// Use the initialState as a default value
export function rootReducer(state: ModConfig = initialState, action: { type: string, value: any }) {
  // The reducer normally looks at the action type field to decide what happens

  // We need to return a new state object always
  if (action.value === false) {
    const newState: ModConfig = {
      ... state,
    };
    delete newState[action.type]
    console.log(newState)
    return newState;
  } else {
    console.log({
      // that has all the existing state data
      ...state,
      // but has a new array for the `todos` field
      [action.type]: action.value
    })
    return {
      // that has all the existing state data
      ...state,
      // but has a new array for the `todos` field
      [action.type]: action.value
    }
  }
}