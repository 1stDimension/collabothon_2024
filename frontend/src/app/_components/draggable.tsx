'use client';

import React from 'react';
import {useDraggable} from '@dnd-kit/core';
import { ReactProps } from '../types';

export function Draggable(props: ReactProps) {
  const {attributes, listeners, setNodeRef, transform} = useDraggable({
    id: 'draggable',
  });
  const style = transform ? {
    transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
  } : undefined;


  return (
    <button ref={setNodeRef} style={style} {...listeners} {...attributes}>
      {props.children}
    </button>
  );
}