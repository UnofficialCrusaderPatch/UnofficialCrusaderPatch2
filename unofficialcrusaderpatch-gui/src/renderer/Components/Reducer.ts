import { ModConfig } from "./Config";

const initialState: ModConfig = {};

// given the input state and an action return a new state based on the action
export function rootReducer(state: ModConfig = initialState, action: { type: string, value: any }) {
  // The reducer normally looks at the action type field to decide what happens

  // We need to return a new state object always
  if (action.value === false) {
    const newState: ModConfig = {
      ... state,
    };
    delete newState[action.type]
    return newState;
  } else {
    return {
      // that has all the existing state data
      ...state,
      // but has a new array for the `todos` field
      [action.type]: action.value
    }
  }
}