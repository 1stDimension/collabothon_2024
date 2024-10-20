'use client';

import React from 'react';
import {useDroppable} from '@dnd-kit/core';
import { ReactProps } from '../types';

export function Droppable(props: ReactProps) {
  const {setNodeRef} = useDroppable({
    id: 'droppable',
  });

  return (
    <div ref={setNodeRef} >
      {props.children}
    </div>
  );
}