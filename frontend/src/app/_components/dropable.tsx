'use client';

import React from 'react';
import {useDroppable} from '@dnd-kit/core';
import { ReactProps } from '../types';

export function Droppable(props: ReactProps) {
  const {isOver, setNodeRef} = useDroppable({
    id: 'droppable',
  });
  const style = {
    color: isOver ? 'green' : undefined,
  };


  return (
    <div ref={setNodeRef} style={style}>
      {props.children}
    </div>
  );
}